using FashionHouse.Application.Features.Products.Command;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Products.Commands
{
    public class ProductAddCommandHandlerTests : HandlerTestBase<ProductAddCommandHandler>
    {
        private static ProductAddCommand CreateCommand() => new()
        {
            CategoryId = Guid.NewGuid(),
            SubCategoryId = Guid.NewGuid(),
            ProductName = "Blue Denim Jacket",
            SKU = "JKT-001",
            Description = "Classic denim jacket",
            Price = 120.50m,
            DiscountedPrice = 99.99m,
            IsActive = true,
            Colors = new List<ProductColor> { ProductColor.Blue },
            Sizes = new List<ProductSize> { ProductSize.M }
        };

        private static Product CreateMappedProduct(ProductAddCommand command) => new()
        {
            CategoryId = command.CategoryId,
            SubCategoryId = command.SubCategoryId,
            ProductName = command.ProductName,
            SKU = command.SKU,
            Description = command.Description,
            Price = command.Price,
            DiscountedPrice = command.DiscountedPrice,
            IsActive = command.IsActive
        };

        [Test]
        public async Task Handle_ValidCommand_AddsProduct()
        {
            // Arrange
            var command = CreateCommand();
            var product = CreateMappedProduct(command);
            var before = DateTime.UtcNow;

            _mapper.Setup(x => x.Map<Product>(command)).Returns(product);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldBeSameAs(product),
                () => result.Id.ShouldNotBe(Guid.Empty),
                () => result.ProductName.ShouldBe(command.ProductName),
                () => result.SKU.ShouldBe(command.SKU),
                () => result.Price.ShouldBe(command.Price),
                () => result.DiscountedPrice.ShouldBe(command.DiscountedPrice),
                () => result.CreatedAt.ShouldBeGreaterThanOrEqualTo(before),
                () => result.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow));

            _productRepository.Verify(x => x.AddAsync(product, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_DuplicateAndUnorderedColorsAndSizes_StoresDistinctOrderedValues()
        {
            // Arrange
            var command = CreateCommand();
            command.Colors = new List<ProductColor>
            {
                ProductColor.Blue, ProductColor.Black, ProductColor.Blue, ProductColor.Red
            };
            command.Sizes = new List<ProductSize>
            {
                ProductSize.XL, ProductSize.S, ProductSize.XL, ProductSize.M
            };

            var product = CreateMappedProduct(command);
            _mapper.Setup(x => x.Map<Product>(command)).Returns(product);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Colors.ShouldBe(new List<ProductColor>
                {
                    ProductColor.Black, ProductColor.Red, ProductColor.Blue
                }),
                () => result.Sizes.ShouldBe(new List<ProductSize>
                {
                    ProductSize.S, ProductSize.M, ProductSize.XL
                }));
        }

        [Test]
        public async Task Handle_NoColorsOrSizes_StoresEmptyLists()
        {
            // Arrange
            var command = CreateCommand();
            command.Colors = new List<ProductColor>();
            command.Sizes = new List<ProductSize>();

            var product = CreateMappedProduct(command);
            _mapper.Setup(x => x.Map<Product>(command)).Returns(product);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Colors.ShouldBeEmpty(),
                () => result.Sizes.ShouldBeEmpty());
        }
    }
}
