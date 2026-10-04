using FashionHouse.Application.Features.Categories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Categories.Queries
{
    public class GetAllCategoriesByPagingQueryHandlerTests : HandlerTestBase<GetAllCategoriesByPagingQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsPagedResultFromRepository()
        {
            // Arrange
            var query = new GetAllCategoriesByPagingQuery { PageIndex = 1, PageSize = 10, SearchText = "shirt", SortText = "Name asc" };
            IList<Category> items = new List<Category> { new Category { Id = Guid.NewGuid() } };
            (IList<Category>, int, int) expected = (items, 25, 12);

            _categoryRepository.Setup(x => x.GetPagedCategories(query, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Item1.ShouldBeSameAs(items),
                () => result.Item2.ShouldBe(25),
                () => result.Item3.ShouldBe(12));

            _categoryRepository.Verify(x => x.GetPagedCategories(query, _token), Times.Once());
        }

        [Test]
        public async Task Handle_NoMatches_ReturnsEmptyPage()
        {
            // Arrange
            var query = new GetAllCategoriesByPagingQuery { PageIndex = 1, PageSize = 10, SearchText = "shirt", SortText = "Name asc" };
            IList<Category> items = new List<Category>();
            (IList<Category>, int, int) expected = (items, 0, 0);

            _categoryRepository.Setup(x => x.GetPagedCategories(query, _token)).ReturnsAsync(expected);

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
