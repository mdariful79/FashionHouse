using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;

namespace FashionHouse.Application.Features.Inventories.Command
{
    public class InventoryAddCommandHandler : ICommandHandler<InventoryAddCommand, Inventory>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public InventoryAddCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Inventory> Handle(InventoryAddCommand command, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.InventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);

            if (existing is not null)
                throw new DuplicateDataException("This product already has a stock record.");

            var inventory = new Inventory
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                ProductId = command.ProductId,
                Quantity = command.Quantity,
                ReorderLevel = command.ReorderLevel,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.InventoryRepository.AddAsync(inventory, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return inventory;
        }
    }
}