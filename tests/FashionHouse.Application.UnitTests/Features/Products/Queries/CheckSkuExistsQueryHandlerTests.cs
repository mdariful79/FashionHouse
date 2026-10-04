using FashionHouse.Application.Features.Products.Query;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Products.Queries
{
    public class CheckSkuExistsQueryHandlerTests : HandlerTestBase<CheckSkuExistsQueryHandler>
    {
        [TestCase(true)]
        [TestCase(false)]
        public async Task Handle_WithExcludeId_ReturnsRepositoryResult(bool isDuplicate)
        {
            // Arrange
            var query = new CheckSkuExistsQuery { SKU = "TSH-100", ExcludeId = Guid.NewGuid() };

            _productRepository
                .Setup(x => x.IsDuplicateSku(query.SKU, query.ExcludeId, _token))
                .ReturnsAsync(isDuplicate);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBe(isDuplicate);
            _productRepository.Verify(x => x.IsDuplicateSku(query.SKU, query.ExcludeId, _token), Times.Once());
        }

        [Test]
        public async Task Handle_WithoutExcludeId_ChecksAgainstAllProducts()
        {
            // Arrange
            var query = new CheckSkuExistsQuery { SKU = "TSH-100", ExcludeId = null };

            _productRepository
                .Setup(x => x.IsDuplicateSku(query.SKU, null, _token))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeTrue();
            _productRepository.Verify(x => x.IsDuplicateSku(query.SKU, null, _token), Times.Once());
        }
    }
}
