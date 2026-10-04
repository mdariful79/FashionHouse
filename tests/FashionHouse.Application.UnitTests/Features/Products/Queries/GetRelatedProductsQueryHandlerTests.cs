using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Products.Queries
{
    public class GetRelatedProductsQueryHandlerTests : HandlerTestBase<GetRelatedProductsQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsItemsFromRepository()
        {
            // Arrange
            var query = new GetRelatedProductsQuery { CategoryId = Guid.NewGuid(), ExcludeProductId = Guid.NewGuid(), Take = 6 };
            IList<Product> expected = new List<Product>
            {
                new Product { Id = Guid.NewGuid() },
                new Product { Id = Guid.NewGuid() }
            };

            _productRepository.Setup(x => x.GetRelatedAsync(query.CategoryId, query.ExcludeProductId, query.Take, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productRepository.Verify(x => x.GetRelatedAsync(query.CategoryId, query.ExcludeProductId, query.Take, _token), Times.Once());
        }

        [Test]
        public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetRelatedProductsQuery { CategoryId = Guid.NewGuid(), ExcludeProductId = Guid.NewGuid(), Take = 6 };
            IList<Product> expected = new List<Product>();

            _productRepository.Setup(x => x.GetRelatedAsync(query.CategoryId, query.ExcludeProductId, query.Take, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
