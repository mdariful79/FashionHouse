using FashionHouse.Application.Features.ProductImages.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.ProductImages.Commands
{
    public class AddProductImageCommandHandlerTests : HandlerTestBase<AddProductImageCommandHandler>
    {
        [Test]
        public async Task Handle_PrimaryImage_ClearsExistingPrimaryFlagBeforeAdding()
        {
            // Arrange
            var command = new AddProductImageCommand
            {
                ProductId = Guid.NewGuid(),
                ImageUrl = "front.jpg",
                IsPrimary = true,
                DisplayOrder = 1
            };
            var calls = new List<string>();

            _productImageRepository
                .Setup(x => x.ClearPrimaryFlagAsync(command.ProductId, _token))
                .Callback(() => calls.Add("clear"))
                .Returns(Task.CompletedTask);
            _productImageRepository
                .Setup(x => x.AddAsync(It.IsAny<ProductImage>(), _token))
                .Callback(() => calls.Add("add"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldNotBe(Guid.Empty),
                () => result.ProductId.ShouldBe(command.ProductId),
                () => result.ImageUrl.ShouldBe("front.jpg"),
                () => result.IsPrimary.ShouldBeTrue(),
                () => result.DisplayOrder.ShouldBe(1));

            calls.ShouldBe(new[] { "clear", "add" });
            _productImageRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_NonPrimaryImage_DoesNotClearPrimaryFlag()
        {
            // Arrange
            var command = new AddProductImageCommand
            {
                ProductId = Guid.NewGuid(),
                ImageUrl = "side.jpg",
                IsPrimary = false,
                DisplayOrder = 2
            };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsPrimary.ShouldBeFalse(),
                () => result.ImageUrl.ShouldBe("side.jpg"),
                () => result.DisplayOrder.ShouldBe(2));

            _productImageRepository.Verify(
                x => x.ClearPrimaryFlagAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _productImageRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }
    }
}
