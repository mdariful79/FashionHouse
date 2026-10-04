using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Inventories.Queries
{
    public class GetActiveProductsWithoutInventoryQueryHandlerTests : HandlerTestBase<GetActiveProductsWithoutInventoryQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsItemsFromRepository()
        {
            // Arrange
            var query = new GetActiveProductsWithoutInventoryQuery {  };
            IList<Product> expected = new List<Product>
            {
                new Product { Id = Guid.NewGuid() },
                new Product { Id = Guid.NewGuid() }
            };

            _productRepository.Setup(x => x.GetActiveWithoutInventoryAsync(_token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productRepository.Verify(x => x.GetActiveWithoutInventoryAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetActiveProductsWithoutInventoryQuery {  };
            IList<Product> expected = new List<Product>();

            _productRepository.Setup(x => x.GetActiveWithoutInventoryAsync(_token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
