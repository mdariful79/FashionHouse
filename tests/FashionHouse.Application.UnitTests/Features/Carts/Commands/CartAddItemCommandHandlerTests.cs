using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Carts.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Carts.Commands
{
    public class CartAddItemCommandHandlerTests : HandlerTestBase<CartAddItemCommandHandler>
    {
        private Guid _customerId;
        private Guid _productId;
        private Cart _cart = null!;

        [SetUp]
        public void Setup()
        {
            _customerId = Guid.NewGuid();
            _productId = Guid.NewGuid();
            _cart = new Cart
            {
                Id = Guid.NewGuid(),
                CustomerId = _customerId,
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            _cartRepository
                .Setup(x => x.GetOrCreateForCustomerAsync(_customerId, _token))
                .ReturnsAsync(_cart);
        }

        private void SetupStock(int? quantity)
        {
            Inventory? inventory = quantity is null
                ? null
                : new Inventory { Id = Guid.NewGuid(), ProductId = _productId, Quantity = quantity.Value };

            _inventoryRepository
                .Setup(x => x.GetByProductIdAsync(_productId, _token))
                .ReturnsAsync(inventory);
        }

        [Test]
        public async Task Handle_ProductNotInCart_AddsNewCartItem()
        {
            // Arrange
            SetupStock(10);
            var command = new CartAddItemCommand { CustomerId = _customerId, ProductId = _productId, Quantity = 2 };
            var before = DateTime.UtcNow;

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldBeSameAs(_cart);
            result.CartItems.Count.ShouldBe(1);

            var item = result.CartItems.Single();
            item.ShouldSatisfyAllConditions(
                () => item.Id.ShouldNotBe(Guid.Empty),
                () => item.CartId.ShouldBe(_cart.Id),
                () => item.ProductId.ShouldBe(_productId),
                () => item.Quantity.ShouldBe(2));

            result.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_ProductAlreadyInCart_IncreasesQuantityInsteadOfAddingDuplicate()
        {
            // Arrange
            SetupStock(10);
            _cart.CartItems.Add(new CartItem { Id = Guid.NewGuid(), CartId = _cart.Id, ProductId = _productId, Quantity = 3 });
            var command = new CartAddItemCommand { CustomerId = _customerId, ProductId = _productId, Quantity = 4 };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.CartItems.Count.ShouldBe(1);
            result.CartItems.Single().Quantity.ShouldBe(7);
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_QuantityEqualsAvailableStock_IsAllowed()
        {
            // Arrange
            SetupStock(5);
            var command = new CartAddItemCommand { CustomerId = _customerId, ProductId = _productId, Quantity = 5 };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.CartItems.Single().Quantity.ShouldBe(5);
        }

        [Test]
        public async Task Handle_RequestedTotalExceedsStock_ThrowsInsufficientStockException()
        {
            // Arrange
            SetupStock(10);
            var existing = new CartItem { Id = Guid.NewGuid(), CartId = _cart.Id, ProductId = _productId, Quantity = 3 };
            _cart.CartItems.Add(existing);
            var command = new CartAddItemCommand { CustomerId = _customerId, ProductId = _productId, Quantity = 8 };

            // Act
            var ex = await Should.ThrowAsync<InsufficientStockException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Only 10 units available in stock.");
            existing.Quantity.ShouldBe(3);
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_NoInventoryRecord_TreatsStockAsZeroAndThrows()
        {
            // Arrange
            SetupStock(null);
            var command = new CartAddItemCommand { CustomerId = _customerId, ProductId = _productId, Quantity = 1 };

            // Act
            var ex = await Should.ThrowAsync<InsufficientStockException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Only 0 units available in stock.");
            _cart.CartItems.ShouldBeEmpty();
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_Success_DoesNotCallEditBecauseCartIsAlreadyTracked()
        {
            // Arrange
            SetupStock(10);
            var command = new CartAddItemCommand { CustomerId = _customerId, ProductId = _productId, Quantity = 1 };

            // Act
            await _handler.Handle(command, _token);

            // Assert
            _cartRepository.Verify(x => x.EditAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
