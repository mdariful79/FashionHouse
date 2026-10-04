using FashionHouse.Application.Features.Inventories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Inventories.Queries
{
    public class GetAllInventoriesByPagingQueryHandlerTests : HandlerTestBase<GetAllInventoriesByPagingQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsPagedResultFromRepository()
        {
            // Arrange
            var query = new GetAllInventoriesByPagingQuery { PageIndex = 1, PageSize = 10, SearchText = "shirt", SortText = "Name asc", LowStockOnly = true };
            IList<Inventory> items = new List<Inventory> { new Inventory { Id = Guid.NewGuid() } };
            (IList<Inventory>, int, int) expected = (items, 25, 12);

            _inventoryRepository.Setup(x => x.GetPagedInventoriesAsync(query, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Item1.ShouldBeSameAs(items),
                () => result.Item2.ShouldBe(25),
                () => result.Item3.ShouldBe(12));

            _inventoryRepository.Verify(x => x.GetPagedInventoriesAsync(query, _token), Times.Once());
        }

        [Test]
        public async Task Handle_NoMatches_ReturnsEmptyPage()
        {
            // Arrange
            var query = new GetAllInventoriesByPagingQuery { PageIndex = 1, PageSize = 10, SearchText = "shirt", SortText = "Name asc", LowStockOnly = true };
            IList<Inventory> items = new List<Inventory>();
            (IList<Inventory>, int, int) expected = (items, 0, 0);

            _inventoryRepository.Setup(x => x.GetPagedInventoriesAsync(query, _token)).ReturnsAsync(expected);

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
