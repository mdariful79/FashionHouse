using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Enums;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Products.Queries
{
    public class GetShopFilterOptionsQueryHandlerTests : HandlerTestBase<GetShopFilterOptionsQueryHandler>
    {
        private static ShopFilterOptionsDto CreateDto() => new()
        {
            Categories = new List<ShopFilterItemDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Men", Count = 12 }
            },
            SubCategories = new List<ShopFilterItemDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Shirts", Count = 5 }
            },
            Colors = new List<ProductColor> { ProductColor.Black, ProductColor.Blue },
            Sizes = new List<ProductSize> { ProductSize.M, ProductSize.L }
        };

        [Test]
        public async Task Handle_WithCategoryId_ReturnsOptionsForThatCategory()
        {
            // Arrange
            var query = new GetShopFilterOptionsQuery { CategoryId = Guid.NewGuid() };
            var expected = CreateDto();

            _productRepository
                .Setup(x => x.GetShopFilterOptionsAsync(query.CategoryId, _token))
                .ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productRepository.Verify(x => x.GetShopFilterOptionsAsync(query.CategoryId, _token), Times.Once());
        }

        [Test]
        public async Task Handle_WithoutCategoryId_ReturnsOptionsForAllCategories()
        {
            // Arrange
            var query = new GetShopFilterOptionsQuery { CategoryId = null };
            var expected = CreateDto();

            _productRepository
                .Setup(x => x.GetShopFilterOptionsAsync(null, _token))
                .ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, _token);

            // Assert
            result.ShouldBeSameAs(expected);
            _productRepository.Verify(x => x.GetShopFilterOptionsAsync(null, _token), Times.Once());
        }
    }
}
