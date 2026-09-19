using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;

namespace FashionHouse.Application.Features.Inventories.Command
{
    public class InventoryAdjustStockCommandHandler : ICommandHandler<InventoryAdjustStockCommand, Inventory>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public InventoryAdjustStockCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Inventory> Handle(InventoryAdjustStockCommand command, CancellationToken cancellationToken)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);

            if (inventory is null)
            {
                // First time this product gets a stock record
                inventory = new Inventory
                {
                    Id = IdentityGenerator.NewSequentialGuid(),
                    ProductId = command.ProductId,
                    Quantity = 0,
                    ReorderLevel = command.ReorderLevel ?? 5
                };

                await _unitOfWork.InventoryRepository.AddAsync(inventory, cancellationToken);
            }

            var newQuantity = inventory.Quantity + command.ChangeQuantity;

            if (newQuantity < 0)
                throw new InsufficientStockException(
                    $"Cannot remove {Math.Abs(command.ChangeQuantity)} units — only {inventory.Quantity} in stock.");

            inventory.Quantity = newQuantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            if (command.ReorderLevel.HasValue)
                inventory.ReorderLevel = command.ReorderLevel.Value;

            await _unitOfWork.InventoryRepository.EditAsync(inventory, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return inventory;
        }
    }
}