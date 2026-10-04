using FashionHouse.Application.Features.SubCategories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.SubCategories.Queries
{
    public class GetAllSubCategoriesByPagingQueryHandlerTests : HandlerTestBase<GetAllSubCategoriesByPagingQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsPagedResultFromRepository()
        {
            // Arrange
            var query = new GetAllSubCategoriesByPagingQuery { PageIndex = 1, PageSize = 10, SearchText = "shirt", SortText = "Name asc" };
            IList<SubCategory> items = new List<SubCategory> { new SubCategory { Id = Guid.NewGuid() } };
            (IList<SubCategory>, int, int) expected = (items, 25, 12);

            _subCategoryRepository.Setup(x => x.GetPagedSubCategories(query, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Item1.ShouldBeSameAs(items),
                () => result.Item2.ShouldBe(25),
                () => result.Item3.ShouldBe(12));

            _subCategoryRepository.Verify(x => x.GetPagedSubCategories(query, _token), Times.Once());
        }

        [Test]
        public async Task Handle_NoMatches_ReturnsEmptyPage()
        {
            // Arrange
            var query = new GetAllSubCategoriesByPagingQuery { PageIndex = 1, PageSize = 10, SearchText = "shirt", SortText = "Name asc" };
            IList<SubCategory> items = new List<SubCategory>();
            (IList<SubCategory>, int, int) expected = (items, 0, 0);

            _subCategoryRepository.Setup(x => x.GetPagedSubCategories(query, _token)).ReturnsAsync(expected);

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
