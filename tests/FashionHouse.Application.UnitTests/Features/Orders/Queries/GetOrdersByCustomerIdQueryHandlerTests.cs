using FashionHouse.Application.Features.Orders.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Orders.Queries
{
    public class GetOrdersByCustomerIdQueryHandlerTests : HandlerTestBase<GetOrdersByCustomerIdQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsItemsFromRepository()
        {
            // Arrange
            var query = new GetOrdersByCustomerIdQuery { CustomerId = Guid.NewGuid() };
            IList<Order> expected = new List<Order>
            {
                new Order { Id = Guid.NewGuid() },
                new Order { Id = Guid.NewGuid() }
            };

            _orderRepository.Setup(x => x.GetByCustomerIdAsync(query.CustomerId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _orderRepository.Verify(x => x.GetByCustomerIdAsync(query.CustomerId, _token), Times.Once());
        }

        [Test]
        public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetOrdersByCustomerIdQuery { CustomerId = Guid.NewGuid() };
            IList<Order> expected = new List<Order>();

            _orderRepository.Setup(x => x.GetByCustomerIdAsync(query.CustomerId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
