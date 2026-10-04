using FashionHouse.Application.Features.Categories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Categories.Queries
{
    public class GetActiveCategoriesQueryHandlerTests : HandlerTestBase<GetActiveCategoriesQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsItemsFromRepository()
        {
            // Arrange
            var query = new GetActiveCategoriesQuery {  };
            IList<Category> expected = new List<Category>
            {
                new Category { Id = Guid.NewGuid() },
                new Category { Id = Guid.NewGuid() }
            };

            _categoryRepository.Setup(x => x.GetActiveAsync(_token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _categoryRepository.Verify(x => x.GetActiveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetActiveCategoriesQuery {  };
            IList<Category> expected = new List<Category>();

            _categoryRepository.Setup(x => x.GetActiveAsync(_token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
