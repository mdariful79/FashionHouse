using FashionHouse.Application.Features.ProductImages.Query;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.ProductImages.Queries
{
    public class GetPrimaryImagesByProductIdsQueryHandlerTests : HandlerTestBase<GetPrimaryImagesByProductIdsQueryHandler>
    {
        [Test]
        public async Task Handle_ProductIdsProvided_ReturnsPrimaryImagesKeyedByProductId()
        {
            // Arrange
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();
            var query = new GetPrimaryImagesByProductIdsQuery { ProductIds = new List<Guid> { productA, productB } };

            IDictionary<Guid, ProductImage> expected = new Dictionary<Guid, ProductImage>
            {
                [productA] = new() { Id = Guid.NewGuid(), ProductId = productA, IsPrimary = true },
                [productB] = new() { Id = Guid.NewGuid(), ProductId = productB, IsPrimary = true }
            };

            _productImageRepository
                .Setup(x => x.GetPrimaryImagesByProductIdsAsync(query.ProductIds, _token))
                .ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productImageRepository.Verify(
                x => x.GetPrimaryImagesByProductIdsAsync(query.ProductIds, _token), Times.Once());
        }

        [Test]
        public async Task Handle_DefaultQuery_ReturnsEmptyDictionary()
        {
            // Arrange
            var query = new GetPrimaryImagesByProductIdsQuery();
            IDictionary<Guid, ProductImage> expected = new Dictionary<Guid, ProductImage>();

            _productImageRepository
                .Setup(x => x.GetPrimaryImagesByProductIdsAsync(query.ProductIds, _token))
                .ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
