using FashionHouse.Application.Features.ProductImages.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.ProductImages.Queries
{
    public class GetProductImagesByProductIdQueryHandlerTests : HandlerTestBase<GetProductImagesByProductIdQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsItemsFromRepository()
        {
            // Arrange
            var query = new GetProductImagesByProductIdQuery { ProductId = Guid.NewGuid() };
            IList<ProductImage> expected = new List<ProductImage>
            {
                new ProductImage { Id = Guid.NewGuid() },
                new ProductImage { Id = Guid.NewGuid() }
            };

            _productImageRepository.Setup(x => x.GetByProductIdAsync(query.ProductId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productImageRepository.Verify(x => x.GetByProductIdAsync(query.ProductId, _token), Times.Once());
        }

        [Test]
        public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetProductImagesByProductIdQuery { ProductId = Guid.NewGuid() };
            IList<ProductImage> expected = new List<ProductImage>();

            _productImageRepository.Setup(x => x.GetByProductIdAsync(query.ProductId, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
