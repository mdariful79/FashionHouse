using FashionHouse.Application.Contracts.Services;
using FashionHouse.Application.Features.ProductImages.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.ProductImages.Commands
{
    public class DeleteProductImageCommandHandlerTests : HandlerTestBase<DeleteProductImageCommandHandler>
    {
        private Mock<IFileStorageService> _fileStorageService = null!;

        [SetUp]
        public void Setup()
        {
            // AutoMock hands the handler this same mock instance.
            _fileStorageService = _autoMock.Mock<IFileStorageService>();
        }

        [Test]
        public async Task Handle_ImageExists_DeletesFileThenRecordThenSaves()
        {
            // Arrange
            var command = new DeleteProductImageCommand { Id = Guid.NewGuid() };
            var image = new ProductImage { Id = command.Id, ImageUrl = "front.jpg" };
            var calls = new List<string>();

            _productImageRepository.Setup(x => x.GetById(command.Id)).Returns(image);
            _fileStorageService
                .Setup(x => x.DeleteImageAsync("front.jpg", "products", _token))
                .Callback(() => calls.Add("file"))
                .Returns(Task.CompletedTask);
            _productImageRepository
                .Setup(x => x.RemoveAsync(command.Id, _token))
                .Callback(() => calls.Add("remove"))
                .Returns(Task.CompletedTask);
            _unitOfWork
                .Setup(x => x.SaveAsync(_token))
                .Callback(() => calls.Add("save"))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, _token);

            // Assert
            calls.ShouldBe(new[] { "file", "remove", "save" });
        }

        [Test]
        public async Task Handle_ImageNotFound_DoesNothing()
        {
            // Arrange
            var command = new DeleteProductImageCommand { Id = Guid.NewGuid() };

            _productImageRepository.Setup(x => x.GetById(command.Id)).Returns((ProductImage)null!);

            // Act
            await _handler.Handle(command, _token);

            // Assert
            _fileStorageService.Verify(
                x => x.DeleteImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never());
            _productImageRepository.Verify(
                x => x.RemoveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_FileDeletionFails_DoesNotRemoveRecordOrSave()
        {
            // Arrange
            var command = new DeleteProductImageCommand { Id = Guid.NewGuid() };
            var image = new ProductImage { Id = command.Id, ImageUrl = "front.jpg" };

            _productImageRepository.Setup(x => x.GetById(command.Id)).Returns(image);
            _fileStorageService
                .Setup(x => x.DeleteImageAsync("front.jpg", "products", _token))
                .ThrowsAsync(new IOException("disk error"));

            // Act & Assert
            await Should.ThrowAsync<IOException>(async () => await _handler.Handle(command, _token));

            _productImageRepository.Verify(
                x => x.RemoveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
