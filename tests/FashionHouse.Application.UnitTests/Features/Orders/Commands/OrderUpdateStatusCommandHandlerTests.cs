using FashionHouse.Application.Features.Orders.Command;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Orders.Commands
{
    public class OrderUpdateStatusCommandHandlerTests : HandlerTestBase<OrderUpdateStatusCommandHandler>
    {
        private Order SetupOrder(OrderStatus status, params (Guid productId, int quantity)[] items)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Status = status,
                PaymentStatus = PaymentStatus.Pending
            };

            foreach (var (productId, quantity) in items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            _orderRepository.Setup(x => x.GetByIdWithItemsAsync(order.Id, _token)).ReturnsAsync(order);
            return order;
        }

        private Inventory SetupInventory(Guid productId, int quantity)
        {
            var inventory = new Inventory { Id = Guid.NewGuid(), ProductId = productId, Quantity = quantity };
            _inventoryRepository.Setup(x => x.GetByProductIdAsync(productId, _token)).ReturnsAsync(inventory);
            return inventory;
        }

        [Test]
        public async Task Handle_ValidCommand_UpdatesStatusPaymentStatusAndTimestamp()
        {
            // Arrange
            var order = SetupOrder(OrderStatus.Pending);
            var command = new OrderUpdateStatusCommand
            {
                Id = order.Id,
                Status = OrderStatus.Shipped,
                PaymentStatus = PaymentStatus.Paid
            };
            var before = DateTime.UtcNow;

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldBeSameAs(order),
                () => result.Status.ShouldBe(OrderStatus.Shipped),
                () => result.PaymentStatus.ShouldBe(PaymentStatus.Paid),
                () => result.UpdatedAt.ShouldNotBeNull(),
                () => result.UpdatedAt!.Value.ShouldBeGreaterThanOrEqualTo(before));

            _orderRepository.Verify(x => x.EditAsync(order, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_OrderNotFound_ThrowsAndDoesNotSave()
        {
            // Arrange
            var command = new OrderUpdateStatusCommand { Id = Guid.NewGuid(), Status = OrderStatus.Confirmed };

            _orderRepository
                .Setup(x => x.GetByIdWithItemsAsync(command.Id, _token))
                .ReturnsAsync((Order?)null);

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Order not found.");
            _orderRepository.Verify(x => x.EditAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [TestCase(OrderStatus.Pending, OrderStatus.Cancelled)]
        [TestCase(OrderStatus.Confirmed, OrderStatus.Cancelled)]
        [TestCase(OrderStatus.Shipped, OrderStatus.Returned)]
        [TestCase(OrderStatus.Delivered, OrderStatus.Returned)]
        public async Task Handle_MovingIntoCancelledOrReturned_RestocksEveryOrderItem(OrderStatus current, OrderStatus target)
        {
            // Arrange
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();
            var order = SetupOrder(current, (productA, 2), (productB, 3));
            var stockA = SetupInventory(productA, 5);
            var stockB = SetupInventory(productB, 0);

            var command = new OrderUpdateStatusCommand
            {
                Id = order.Id,
                Status = target,
                PaymentStatus = PaymentStatus.Refunded
            };
            var before = DateTime.UtcNow;

            // Act
            await _handler.Handle(command, _token);

            // Assert
            stockA.Quantity.ShouldBe(7);   // 5 + 2
            stockB.Quantity.ShouldBe(3);   // 0 + 3
            stockA.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);

            _inventoryRepository.Verify(x => x.EditAsync(stockA, _token), Times.Once());
            _inventoryRepository.Verify(x => x.EditAsync(stockB, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [TestCase(OrderStatus.Cancelled, OrderStatus.Cancelled)]
        [TestCase(OrderStatus.Cancelled, OrderStatus.Returned)]
        [TestCase(OrderStatus.Returned, OrderStatus.Cancelled)]
        [TestCase(OrderStatus.Cancelled, OrderStatus.Pending)]
        [TestCase(OrderStatus.Pending, OrderStatus.Shipped)]
        [TestCase(OrderStatus.Shipped, OrderStatus.Delivered)]
        public async Task Handle_NotATransitionIntoCancelledOrReturned_DoesNotRestock(OrderStatus current, OrderStatus target)
        {
            // Arrange
            var productA = Guid.NewGuid();
            var order = SetupOrder(current, (productA, 2));
            var stockA = SetupInventory(productA, 5);

            var command = new OrderUpdateStatusCommand { Id = order.Id, Status = target };

            // Act
            await _handler.Handle(command, _token);

            // Assert
            stockA.Quantity.ShouldBe(5);
            _inventoryRepository.Verify(
                x => x.GetByProductIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _inventoryRepository.Verify(
                x => x.EditAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_CancellingOrderWhoseProductHasNoInventory_SkipsThatItemAndStillUpdatesOrder()
        {
            // Arrange
            var productA = Guid.NewGuid();
            var order = SetupOrder(OrderStatus.Pending, (productA, 2));

            _inventoryRepository
                .Setup(x => x.GetByProductIdAsync(productA, _token))
                .ReturnsAsync((Inventory?)null);

            var command = new OrderUpdateStatusCommand { Id = order.Id, Status = OrderStatus.Cancelled };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.Status.ShouldBe(OrderStatus.Cancelled);
            _inventoryRepository.Verify(
                x => x.EditAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()), Times.Never());
            _orderRepository.Verify(x => x.EditAsync(order, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }
    }
}
