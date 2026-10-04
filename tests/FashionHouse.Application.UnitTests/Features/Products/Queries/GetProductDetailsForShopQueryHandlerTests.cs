using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Products.Queries
{
    public class GetProductDetailsForShopQueryHandlerTests : HandlerTestBase<GetProductDetailsForShopQueryHandler>
    {
        [Test]
        public async Task Handle_EntityExists_ReturnsEntity()
        {
            // Arrange
            var query = new GetProductDetailsForShopQuery { Id = Guid.NewGuid() };
            var expected = new Product { Id = Guid.NewGuid() };

            _productRepository.Setup(x => x.GetByIdWithDetailsAsync(query.Id, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productRepository.Verify(x => x.GetByIdWithDetailsAsync(query.Id, _token), Times.Once());
        }

        [Test]
        public async Task Handle_EntityDoesNotExist_ReturnsNull()
        {
            // Arrange
            var query = new GetProductDetailsForShopQuery { Id = Guid.NewGuid() };

            _productRepository.Setup(x => x.GetByIdWithDetailsAsync(query.Id, _token)).ReturnsAsync((Product?)null);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeNull();
        }
    }
}
