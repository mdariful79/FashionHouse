using FashionHouse.Application.Features.Categories.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Categories.Queries
{
    public class GetCategoryByIdQueryHandlerTests : HandlerTestBase<GetCategoryByIdQueryHandler>
    {
        [Test]
        public async Task Handle_EntityExists_ReturnsEntity()
        {
            // Arrange
            var query = new GetCategoryByIdQuery { Id = Guid.NewGuid() };
            var expected = new Category { Id = Guid.NewGuid() };

            _categoryRepository.Setup(x => x.GetByIdAsync(query.Id, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _categoryRepository.Verify(x => x.GetByIdAsync(query.Id, _token), Times.Once());
        }

        [Test]
        public async Task Handle_EntityDoesNotExist_ReturnsNull()
        {
            // Arrange
            var query = new GetCategoryByIdQuery { Id = Guid.NewGuid() };

            _categoryRepository.Setup(x => x.GetByIdAsync(query.Id, _token)).ReturnsAsync((Category?)null);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeNull();
        }
    }
}
