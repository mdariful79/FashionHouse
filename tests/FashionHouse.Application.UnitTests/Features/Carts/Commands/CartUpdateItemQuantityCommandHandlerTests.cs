using FashionHouse.Application.Features.Carts.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Carts.Commands
{
    public class CartUpdateItemQuantityCommandHandlerTests : HandlerTestBase<CartUpdateItemQuantityCommandHandler>
    {
        private Guid _customerId;
        private Cart _cart = null!;
        private CartItem _item = null!;

        [SetUp]
        public void Setup()
        {
            _customerId = Guid.NewGuid();
            _cart = new Cart { Id = Guid.NewGuid(), CustomerId = _customerId, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
            _item = new CartItem { Id = Guid.NewGuid(), CartId = _cart.Id, ProductId = Guid.NewGuid(), Quantity = 2 };
            _cart.CartItems.Add(_item);

            _cartRepository.Setup(x => x.GetByCustomerIdAsync(_customerId, _token)).ReturnsAsync(_cart);
        }

        [Test]
        public async Task Handle_PositiveQuantity_UpdatesItemQuantity()
        {
            // Arrange
            var command = new CartUpdateItemQuantityCommand { CustomerId = _customerId, CartItemId = _item.Id, Quantity = 6 };
            var before = DateTime.UtcNow;

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldBeTrue();
            _item.Quantity.ShouldBe(6);
            _cart.CartItems.Count.ShouldBe(1);
            _cart.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [TestCase(0)]
        [TestCase(-3)]
        public async Task Handle_ZeroOrNegativeQuantity_RemovesItemFromCart(int quantity)
        {
            // Arrange
            var command = new CartUpdateItemQuantityCommand { CustomerId = _customerId, CartItemId = _item.Id, Quantity = quantity };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldBeTrue();
            _cart.CartItems.ShouldBeEmpty();
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_ItemNotInCart_ThrowsAndDoesNotSave()
        {
            // Arrange
            var command = new CartUpdateItemQuantityCommand { CustomerId = _customerId, CartItemId = Guid.NewGuid(), Quantity = 3 };

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Cart item not found.");
            _item.Quantity.ShouldBe(2);
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_CustomerHasNoCart_ThrowsAndDoesNotSave()
        {
            // Arrange
            var otherCustomerId = Guid.NewGuid();
            _cartRepository
                .Setup(x => x.GetByCustomerIdAsync(otherCustomerId, _token))
                .ReturnsAsync((Cart?)null);
            var command = new CartUpdateItemQuantityCommand { CustomerId = otherCustomerId, CartItemId = _item.Id, Quantity = 3 };

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Cart item not found.");
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
