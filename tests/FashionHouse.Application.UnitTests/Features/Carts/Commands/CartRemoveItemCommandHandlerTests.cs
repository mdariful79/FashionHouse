using FashionHouse.Application.Features.Carts.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Carts.Commands
{
    public class CartRemoveItemCommandHandlerTests : HandlerTestBase<CartRemoveItemCommandHandler>
    {
        [Test]
        public async Task Handle_ItemInCart_RemovesItemAndSaves()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var cart = new Cart { Id = Guid.NewGuid(), CustomerId = customerId, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
            var target = new CartItem { Id = Guid.NewGuid(), CartId = cart.Id, ProductId = Guid.NewGuid(), Quantity = 1 };
            var other = new CartItem { Id = Guid.NewGuid(), CartId = cart.Id, ProductId = Guid.NewGuid(), Quantity = 2 };
            cart.CartItems.Add(target);
            cart.CartItems.Add(other);
            var before = DateTime.UtcNow;

            _cartRepository.Setup(x => x.GetByCustomerIdAsync(customerId, _token)).ReturnsAsync(cart);
            var command = new CartRemoveItemCommand { CustomerId = customerId, CartItemId = target.Id };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldBeTrue();
            cart.CartItems.ShouldBe(new[] { other });
            cart.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_ItemNotInCart_ReturnsTrueWithoutSaving()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var cart = new Cart { Id = Guid.NewGuid(), CustomerId = customerId };
            var existing = new CartItem { Id = Guid.NewGuid(), CartId = cart.Id, Quantity = 1 };
            cart.CartItems.Add(existing);

            _cartRepository.Setup(x => x.GetByCustomerIdAsync(customerId, _token)).ReturnsAsync(cart);
            var command = new CartRemoveItemCommand { CustomerId = customerId, CartItemId = Guid.NewGuid() };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldBeTrue();
            cart.CartItems.Count.ShouldBe(1);
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_CustomerHasNoCart_ReturnsTrueWithoutSaving()
        {
            // Arrange
            var command = new CartRemoveItemCommand { CustomerId = Guid.NewGuid(), CartItemId = Guid.NewGuid() };

            _cartRepository
                .Setup(x => x.GetByCustomerIdAsync(command.CustomerId, _token))
                .ReturnsAsync((Cart?)null);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldBeTrue();
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
