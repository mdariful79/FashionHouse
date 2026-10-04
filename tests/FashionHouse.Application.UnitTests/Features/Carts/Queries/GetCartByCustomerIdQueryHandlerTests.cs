using FashionHouse.Application.Features.Carts.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Carts.Queries
{
    public class GetCartByCustomerIdQueryHandlerTests : HandlerTestBase<GetCartByCustomerIdQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsCartAndSavesSoNewCartIsPersisted()
        {
            // Arrange
            var query = new GetCartByCustomerIdQuery { CustomerId = Guid.NewGuid() };
            var cart = new Cart { Id = Guid.NewGuid(), CustomerId = query.CustomerId };
            var calls = new List<string>();

            _cartRepository
                .Setup(x => x.GetOrCreateForCustomerAsync(query.CustomerId, _token))
                .Callback(() => calls.Add("getOrCreate"))
                .ReturnsAsync(cart);
            _unitOfWork
                .Setup(x => x.SaveAsync(_token))
                .Callback(() => calls.Add("save"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(cart);
            calls.ShouldBe(new[] { "getOrCreate", "save" });
        }
    }
}
