using Cortex.Mediator.Commands;

namespace FashionHouse.Application.Features.Carts.Command
{
    public class CartRemoveItemCommand : ICommand<bool>
    {
        public Guid CustomerId { get; set; }
        public Guid CartItemId { get; set; }
    }
}