using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Home.Queries
{
    public class GetHomeProductsQueryHandlerTests : HandlerTestBase<GetHomeProductsQueryHandler>
    {
        [Test]
        public async Task Handle_CustomTake_PassesTakeToRepository()
        {
            // Arrange
            var query = new GetHomeProductsQuery { Take = 4 };
            var expected = new HomeProductsDto
            {
                BestSellers = new List<Product> { new() { Id = Guid.NewGuid() } },
                NewArrivals = new List<Product> { new() { Id = Guid.NewGuid() } },
                HotSales = new List<Product> { new() { Id = Guid.NewGuid() } }
            };

            _productRepository.Setup(x => x.GetHomeProductsAsync(4, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productRepository.Verify(x => x.GetHomeProductsAsync(4, _token), Times.Once());
        }

        [Test]
        public async Task Handle_DefaultQuery_UsesDefaultTakeOfEight()
        {
            // Arrange
            var query = new GetHomeProductsQuery();
            var expected = new HomeProductsDto();

            _productRepository.Setup(x => x.GetHomeProductsAsync(8, _token)).ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productRepository.Verify(x => x.GetHomeProductsAsync(8, _token), Times.Once());
        }
    }
}
