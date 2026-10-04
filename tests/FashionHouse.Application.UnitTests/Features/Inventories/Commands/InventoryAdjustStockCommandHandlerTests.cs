using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Inventories.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Inventories.Commands
{
    public class InventoryAdjustStockCommandHandlerTests : HandlerTestBase<InventoryAdjustStockCommandHandler>
    {
        private Inventory SetupExistingInventory(Guid productId, int quantity, int reorderLevel = 5)
        {
            var inventory = new Inventory
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = quantity,
                ReorderLevel = reorderLevel,
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            _inventoryRepository
                .Setup(x => x.GetByProductIdAsync(productId, _token))
                .ReturnsAsync(inventory);

            return inventory;
        }

        private void SetupNoInventory(Guid productId) =>
            _inventoryRepository
                .Setup(x => x.GetByProductIdAsync(productId, _token))
                .ReturnsAsync((Inventory?)null);

        [TestCase(10, 5, 15)]
        [TestCase(10, -4, 6)]
        [TestCase(10, -10, 0)]
        [TestCase(0, 3, 3)]
        public async Task Handle_ExistingInventory_AppliesQuantityChange(int current, int change, int expected)
        {
            // Arrange
            var productId = Guid.NewGuid();
            var inventory = SetupExistingInventory(productId, current);
            var command = new InventoryAdjustStockCommand { ProductId = productId, ChangeQuantity = change };
            var before = DateTime.UtcNow;

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldBeSameAs(inventory),
                () => result.Quantity.ShouldBe(expected),
                () => result.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before));

            _inventoryRepository.Verify(x => x.AddAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()), Times.Never());
            _inventoryRepository.Verify(x => x.EditAsync(inventory, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_ReorderLevelProvided_UpdatesReorderLevel()
        {
            // Arrange
            var productId = Guid.NewGuid();
            SetupExistingInventory(productId, quantity: 10, reorderLevel: 5);
            var command = new InventoryAdjustStockCommand { ProductId = productId, ChangeQuantity = 1, ReorderLevel = 20 };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ReorderLevel.ShouldBe(20);
        }

        [Test]
        public async Task Handle_ReorderLevelNotProvided_KeepsExistingReorderLevel()
        {
            // Arrange
            var productId = Guid.NewGuid();
            SetupExistingInventory(productId, quantity: 10, reorderLevel: 7);
            var command = new InventoryAdjustStockCommand { ProductId = productId, ChangeQuantity = 1, ReorderLevel = null };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ReorderLevel.ShouldBe(7);
        }

        [Test]
        public async Task Handle_NoInventoryRecord_CreatesRecordWithDefaultReorderLevelAndAppliesChange()
        {
            // Arrange
            var productId = Guid.NewGuid();
            SetupNoInventory(productId);
            var command = new InventoryAdjustStockCommand { ProductId = productId, ChangeQuantity = 8 };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldNotBe(Guid.Empty),
                () => result.ProductId.ShouldBe(productId),
                () => result.Quantity.ShouldBe(8),
                () => result.ReorderLevel.ShouldBe(5));

            _inventoryRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
            _inventoryRepository.Verify(x => x.EditAsync(result, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_NoInventoryRecordAndReorderLevelProvided_UsesProvidedReorderLevel()
        {
            // Arrange
            var productId = Guid.NewGuid();
            SetupNoInventory(productId);
            var command = new InventoryAdjustStockCommand { ProductId = productId, ChangeQuantity = 8, ReorderLevel = 20 };

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ReorderLevel.ShouldBe(20);
        }

        [Test]
        public async Task Handle_RemovingMoreThanInStock_ThrowsInsufficientStockException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var inventory = SetupExistingInventory(productId, quantity: 3);
            var command = new InventoryAdjustStockCommand { ProductId = productId, ChangeQuantity = -10 };

            // Act
            var ex = await Should.ThrowAsync<InsufficientStockException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldStartWith("Cannot remove 10 units");
            ex.Message.ShouldContain("only 3 in stock");

            inventory.Quantity.ShouldBe(3);
            _inventoryRepository.Verify(x => x.EditAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_NoInventoryRecordAndNegativeChange_ThrowsAndDoesNotSave()
        {
            // Arrange
            var productId = Guid.NewGuid();
            SetupNoInventory(productId);
            var command = new InventoryAdjustStockCommand { ProductId = productId, ChangeQuantity = -1 };

            // Act
            var ex = await Should.ThrowAsync<InsufficientStockException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldContain("only 0 in stock");
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
