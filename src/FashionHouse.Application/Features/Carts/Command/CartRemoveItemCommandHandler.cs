using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.Carts.Command
{
    public class CartRemoveItemCommandHandler : ICommandHandler<CartRemoveItemCommand, bool>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public CartRemoveItemCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CartRemoveItemCommand command, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.CartRepository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);
            var item = cart?.CartItems.FirstOrDefault(x => x.Id == command.CartItemId);

            if (item is not null)
            {
                cart!.CartItems.Remove(item);
                cart.UpdatedAt = DateTime.UtcNow;
                // no EditAsync call here
                await _unitOfWork.SaveAsync(cancellationToken);
            }

            return true;
        }
    }
}