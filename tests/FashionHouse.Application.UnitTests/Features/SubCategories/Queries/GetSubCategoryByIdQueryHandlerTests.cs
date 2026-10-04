using FashionHouse.Application.Features.SubCategories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.SubCategories.Queries
{
    public class GetSubCategoryByIdQueryHandlerTests : HandlerTestBase<GetSubCategoryByIdQueryHandler>
    {
        [Test]
        public async Task Handle_EntityExists_ReturnsEntity()
        {
            // Arrange
            var query = new GetSubCategoryByIdQuery { Id = Guid.NewGuid() };
            var expected = new SubCategory { Id = Guid.NewGuid() };

            _subCategoryRepository.Setup(x => x.GetByIdAsync(query.Id, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _subCategoryRepository.Verify(x => x.GetByIdAsync(query.Id, _token), Times.Once());
        }

        [Test]
        public async Task Handle_EntityDoesNotExist_ReturnsNull()
        {
            // Arrange
            var query = new GetSubCategoryByIdQuery { Id = Guid.NewGuid() };

            _subCategoryRepository.Setup(x => x.GetByIdAsync(query.Id, _token)).ReturnsAsync((SubCategory?)null);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeNull();
        }
    }
}
