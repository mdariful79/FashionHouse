using FashionHouse.Application.Features.Products.Command;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Products.Commands
{
    public class ProductDeleteCommandHandlerTests : HandlerTestBase<ProductDeleteCommandHandler>
    {
        [Test]
        public async Task Handle_ValidCommand_RemovesThenSaves()
        {
            // Arrange
            var command = new ProductDeleteCommand { Id = Guid.NewGuid() };
            var calls = new List<string>();

            _productRepository
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
            calls.ShouldBe(new[] { "remove", "save" });
        }

        [Test]
        public async Task Handle_RemoveFails_DoesNotSave()
        {
            // Arrange
            var command = new ProductDeleteCommand { Id = Guid.NewGuid() };

            _productRepository
                .Setup(x => x.RemoveAsync(command.Id, _token))
                .ThrowsAsync(new InvalidOperationException("remove failed"));

            // Act & Assert
            var ex = await Should.ThrowAsync<InvalidOperationException>(
                async () => await _handler.Handle(command, _token));

            ex.Message.ShouldBe("remove failed");
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
