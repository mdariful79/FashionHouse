using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Inventories.Command
{
    public class InventoryAddCommand : ICommand<Inventory>
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; } = 5;
    }
}