using FashionHouse.Application.Features.Inventories.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Inventories.Commands
{
    public class InventoryUpdateCommandHandlerTests : HandlerTestBase<InventoryUpdateCommandHandler>
    {
        [Test]
        public async Task Handle_InventoryExists_UpdatesQuantityAndReorderLevel()
        {
            // Arrange
            var command = new InventoryUpdateCommand { Id = Guid.NewGuid(), Quantity = 50, ReorderLevel = 12 };
            var inventory = new Inventory
            {
                Id = command.Id,
                Quantity = 5,
                ReorderLevel = 5,
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };
            var before = DateTime.UtcNow;

            _inventoryRepository.Setup(x => x.GetByIdAsync(command.Id, _token)).ReturnsAsync(inventory);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldBeSameAs(inventory),
                () => result.Quantity.ShouldBe(50),
                () => result.ReorderLevel.ShouldBe(12),
                () => result.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before));

            _inventoryRepository.Verify(x => x.EditAsync(inventory, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_InventoryNotFound_ThrowsAndDoesNotSave()
        {
            // Arrange
            var command = new InventoryUpdateCommand { Id = Guid.NewGuid(), Quantity = 1, ReorderLevel = 1 };

            _inventoryRepository
                .Setup(x => x.GetByIdAsync(command.Id, _token))
                .ReturnsAsync((Inventory?)null);

            // Act
            var ex = await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Stock record not found.");

            _inventoryRepository.Verify(x => x.EditAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
