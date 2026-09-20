using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;

namespace FashionHouse.Application.Features.Carts.Command
{
    public class CartUpdateItemQuantityCommandHandler : ICommandHandler<CartUpdateItemQuantityCommand, bool>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public CartUpdateItemQuantityCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CartUpdateItemQuantityCommand command, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.CartRepository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);
            var item = cart?.CartItems.FirstOrDefault(x => x.Id == command.CartItemId);

            if (item is null)
                throw new Exception("Cart item not found.");

            if (command.Quantity <= 0)
            {
                cart!.CartItems.Remove(item);
            }
            else
            {
                // ...stock check...
                item.Quantity = command.Quantity;
            }

            cart!.UpdatedAt = DateTime.UtcNow;
            // no EditAsync call here
            await _unitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}