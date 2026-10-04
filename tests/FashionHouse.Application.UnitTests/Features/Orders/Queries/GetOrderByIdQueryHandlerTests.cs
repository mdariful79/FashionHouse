using FashionHouse.Application.Features.Orders.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Orders.Queries
{
    public class GetOrderByIdQueryHandlerTests : HandlerTestBase<GetOrderByIdQueryHandler>
    {
        [Test]
        public async Task Handle_EntityExists_ReturnsEntity()
        {
            // Arrange
            var query = new GetOrderByIdQuery { Id = Guid.NewGuid() };
            var expected = new Order { Id = Guid.NewGuid() };

            _orderRepository.Setup(x => x.GetByIdWithItemsAsync(query.Id, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _orderRepository.Verify(x => x.GetByIdWithItemsAsync(query.Id, _token), Times.Once());
        }

        [Test]
        public async Task Handle_EntityDoesNotExist_ReturnsNull()
        {
            // Arrange
            var query = new GetOrderByIdQuery { Id = Guid.NewGuid() };

            _orderRepository.Setup(x => x.GetByIdWithItemsAsync(query.Id, _token)).ReturnsAsync((Order?)null);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeNull();
        }
    }
}
