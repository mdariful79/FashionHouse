using FashionHouse.Application.Features.ProductImages.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.ProductImages.Commands
{
    public class SetPrimaryProductImageCommandHandlerTests : HandlerTestBase<SetPrimaryProductImageCommandHandler>
    {
        [Test]
        public async Task Handle_ImageExists_ClearsOldPrimaryAndMarksImageAsPrimary()
        {
            // Arrange
            var command = new SetPrimaryProductImageCommand { Id = Guid.NewGuid(), ProductId = Guid.NewGuid() };
            var image = new ProductImage { Id = command.Id, ProductId = command.ProductId, IsPrimary = false };
            var calls = new List<string>();

            _productImageRepository
                .Setup(x => x.ClearPrimaryFlagAsync(command.ProductId, _token))
                .Callback(() => calls.Add("clear"))
                .Returns(Task.CompletedTask);
            _productImageRepository.Setup(x => x.GetById(command.Id)).Returns(image);
            _unitOfWork
                .Setup(x => x.SaveAsync(_token))
                .Callback(() => calls.Add("save"))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, _token);

            // Assert
            image.IsPrimary.ShouldBeTrue();
            calls.ShouldBe(new[] { "clear", "save" });
        }

        [Test]
        public async Task Handle_ImageNotFound_DoesNotThrowAndStillSaves()
        {
            // Arrange
            var command = new SetPrimaryProductImageCommand { Id = Guid.NewGuid(), ProductId = Guid.NewGuid() };

            _productImageRepository.Setup(x => x.GetById(command.Id)).Returns((ProductImage)null!);

            // Act
            await Should.NotThrowAsync(async () => await _handler.Handle(command, _token));

            // Assert
            _productImageRepository.Verify(x => x.ClearPrimaryFlagAsync(command.ProductId, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }
    }
}
