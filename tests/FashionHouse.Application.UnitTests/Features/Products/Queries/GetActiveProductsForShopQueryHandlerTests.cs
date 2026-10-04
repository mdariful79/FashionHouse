using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Products.Queries
{
    public class GetActiveProductsForShopQueryHandlerTests : HandlerTestBase<GetActiveProductsForShopQueryHandler>
    {
        [Test]
        public async Task Handle_ValidQuery_ReturnsPagedResultFromRepository()
        {
            // Arrange
            var query = new GetActiveProductsForShopQuery { PageIndex = 2, PageSize = 12, SearchText = "shirt", SortBy = "price_asc" };
            IList<Product> items = new List<Product> { new Product { Id = Guid.NewGuid() } };
            (IList<Product>, int, int) expected = (items, 25, 12);

            _productRepository.Setup(x => x.GetActiveForShopAsync(query, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Item1.ShouldBeSameAs(items),
                () => result.Item2.ShouldBe(25),
                () => result.Item3.ShouldBe(12));

            _productRepository.Verify(x => x.GetActiveForShopAsync(query, _token), Times.Once());
        }

        [Test]
        public async Task Handle_NoMatches_ReturnsEmptyPage()
        {
            // Arrange
            var query = new GetActiveProductsForShopQuery { PageIndex = 2, PageSize = 12, SearchText = "shirt", SortBy = "price_asc" };
            IList<Product> items = new List<Product>();
            (IList<Product>, int, int) expected = (items, 0, 0);

            _productRepository.Setup(x => x.GetActiveForShopAsync(query, _token)).ReturnsAsync(expected);

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
