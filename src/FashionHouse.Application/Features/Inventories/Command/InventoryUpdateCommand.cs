using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Inventories.Command
{
    public class InventoryUpdateCommand : ICommand<Inventory>
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
    }
}