using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Inventories.Command
{
    public class InventoryUpdateCommandHandler : ICommandHandler<InventoryUpdateCommand, Inventory>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public InventoryUpdateCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Inventory> Handle(InventoryUpdateCommand command, CancellationToken cancellationToken)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(command.Id, cancellationToken);

            if (inventory is null)
                throw new Exception("Stock record not found.");

            inventory.Quantity = command.Quantity;
            inventory.ReorderLevel = command.ReorderLevel;
            inventory.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.InventoryRepository.EditAsync(inventory, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return inventory;
        }
    }
}