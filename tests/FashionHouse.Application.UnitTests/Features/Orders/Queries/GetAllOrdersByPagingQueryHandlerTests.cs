using FashionHouse.Application.Features.Orders.Query;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Orders.Queries
{
    public class GetAllOrdersByPagingQueryHandlerTests : HandlerTestBase<GetAllOrdersByPagingQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsPagedResultFromRepository()
        {
            // Arrange
            var query = new GetAllOrdersByPagingQuery { PageIndex = 1, PageSize = 10, SearchText = "shirt", SortText = "Name asc", StatusFilter = OrderStatus.Pending };
            IList<Order> items = new List<Order> { new Order { Id = Guid.NewGuid() } };
            (IList<Order>, int, int) expected = (items, 25, 12);

            _orderRepository.Setup(x => x.GetPagedOrdersAsync(query, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Item1.ShouldBeSameAs(items),
                () => result.Item2.ShouldBe(25),
                () => result.Item3.ShouldBe(12));

            _orderRepository.Verify(x => x.GetPagedOrdersAsync(query, _token), Times.Once());
        }

        [Test]
        public async Task Handle_NoMatches_ReturnsEmptyPage()
        {
            // Arrange
            var query = new GetAllOrdersByPagingQuery { PageIndex = 1, PageSize = 10, SearchText = "shirt", SortText = "Name asc", StatusFilter = OrderStatus.Pending };
            IList<Order> items = new List<Order>();
            (IList<Order>, int, int) expected = (items, 0, 0);

            _orderRepository.Setup(x => x.GetPagedOrdersAsync(query, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Item1.ShouldBeEmpty(),
                () => result.Item2.ShouldBe(0),
                () => result.Item3.ShouldBe(0));
        }
    }
}
