using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Products.Command;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Products.Commands
{
    public class ProductUpdateCommandHandlerTests : HandlerTestBase<ProductUpdateCommandHandler>
    {
        private static ProductUpdateCommand CreateCommand() => new()
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            SubCategoryId = Guid.NewGuid(),
            ProductName = "Blue Denim Jacket",
            SKU = "JKT-001",
            Description = "Updated description",
            Price = 130m,
            DiscountedPrice = null,
            IsActive = true,
            Colors = new List<ProductColor> { ProductColor.Blue },
            Sizes = new List<ProductSize> { ProductSize.M }
        };

        [Test]
        public async Task Handle_UniqueSku_UpdatesProduct()
        {
            // Arrange
            var command = CreateCommand();
            var existing = new Product { Id = command.Id, ProductName = "Old", SKU = "OLD-1" };
            var updated = new Product
            {
                Id = command.Id,
                ProductName = command.ProductName,
                SKU = command.SKU,
                Price = command.Price
            };
            var before = DateTime.UtcNow;

            _productRepository.Setup(x => x.IsDuplicateSku(command.SKU, command.Id, _token)).ReturnsAsync(false);
            _productRepository.Setup(x => x.GetById(command.Id)).Returns(existing);
            _mapper.Setup(x => x.Map(command, existing)).Returns(updated);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldBeSameAs(updated),
                () => result.ProductName.ShouldBe(command.ProductName),
                () => result.SKU.ShouldBe(command.SKU),
                () => result.UpdatedAt.ShouldNotBeNull(),
                () => result.UpdatedAt!.Value.ShouldBeGreaterThanOrEqualTo(before));

            _productRepository.Verify(x => x.EditAsync(updated, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_DuplicateAndUnorderedColorsAndSizes_StoresDistinctOrderedValues()
        {
            // Arrange
            var command = CreateCommand();
            command.Colors = new List<ProductColor> { ProductColor.Navy, ProductColor.White, ProductColor.Navy };
            command.Sizes = new List<ProductSize> { ProductSize.XXL, ProductSize.XS, ProductSize.XXL };

            var existing = new Product { Id = command.Id };
            var updated = new Product { Id = command.Id };

            _productRepository.Setup(x => x.GetById(command.Id)).Returns(existing);
            _mapper.Setup(x => x.Map(command, existing)).Returns(updated);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Colors.ShouldBe(new List<ProductColor> { ProductColor.White, ProductColor.Navy }),
                () => result.Sizes.ShouldBe(new List<ProductSize> { ProductSize.XS, ProductSize.XXL }));
        }

        [Test]
        public async Task Handle_DuplicateSku_ThrowsDuplicateDataException()
        {
            // Arrange
            var command = CreateCommand();

            _productRepository.Setup(x => x.IsDuplicateSku(command.SKU, command.Id, _token)).ReturnsAsync(true);

            // Act
            var ex = await Should.ThrowAsync<DuplicateDataException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe($"SKU '{command.SKU}' already exists.");

            _productRepository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never());
            _productRepository.Verify(x => x.EditAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
