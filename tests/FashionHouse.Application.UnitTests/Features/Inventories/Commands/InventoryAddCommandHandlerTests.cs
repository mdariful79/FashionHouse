using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Inventories.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Inventories.Commands
{
    public class InventoryAddCommandHandlerTests : HandlerTestBase<InventoryAddCommandHandler>
    {
        [Test]
        public async Task Handle_ProductHasNoStockRecord_CreatesInventory()
        {
            // Arrange
            var command = new InventoryAddCommand { ProductId = Guid.NewGuid(), Quantity = 40, ReorderLevel = 8 };
            var before = DateTime.UtcNow;

            _inventoryRepository
                .Setup(x => x.GetByProductIdAsync(command.ProductId, _token))
                .ReturnsAsync((Inventory?)null);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldNotBe(Guid.Empty),
                () => result.ProductId.ShouldBe(command.ProductId),
                () => result.Quantity.ShouldBe(40),
                () => result.ReorderLevel.ShouldBe(8),
                () => result.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before));

            _inventoryRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_ProductAlreadyHasStockRecord_ThrowsDuplicateDataException()
        {
            // Arrange
            var command = new InventoryAddCommand { ProductId = Guid.NewGuid(), Quantity = 10 };

            _inventoryRepository
                .Setup(x => x.GetByProductIdAsync(command.ProductId, _token))
                .ReturnsAsync(new Inventory { Id = Guid.NewGuid(), ProductId = command.ProductId });

            // Act
            var ex = await Should.ThrowAsync<DuplicateDataException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("This product already has a stock record.");

            _inventoryRepository.Verify(x => x.AddAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
