using FashionHouse.Application.Features.SubCategories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.SubCategories.Queries
{
    public class GetSubCategoriesByCategoryIdQueryHandlerTests : HandlerTestBase<GetSubCategoriesByCategoryIdQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsItemsFromRepository()
        {
            // Arrange
            var query = new GetSubCategoriesByCategoryIdQuery { CategoryId = Guid.NewGuid() };
            IList<SubCategory> expected = new List<SubCategory>
            {
                new SubCategory { Id = Guid.NewGuid() },
                new SubCategory { Id = Guid.NewGuid() }
            };

            _subCategoryRepository.Setup(x => x.GetByCategoryIdAsync(query.CategoryId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _subCategoryRepository.Verify(x => x.GetByCategoryIdAsync(query.CategoryId, _token), Times.Once());
        }

        [Test]
        public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetSubCategoriesByCategoryIdQuery { CategoryId = Guid.NewGuid() };
            IList<SubCategory> expected = new List<SubCategory>();

            _subCategoryRepository.Setup(x => x.GetByCategoryIdAsync(query.CategoryId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
