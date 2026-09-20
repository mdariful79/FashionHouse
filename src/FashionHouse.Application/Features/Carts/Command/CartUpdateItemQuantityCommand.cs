using Cortex.Mediator.Commands;

namespace FashionHouse.Application.Features.Carts.Command
{
    public class CartUpdateItemQuantityCommand : ICommand<bool>
    {
        public Guid CustomerId { get; set; }
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}