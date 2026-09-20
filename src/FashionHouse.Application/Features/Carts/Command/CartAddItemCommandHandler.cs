using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;

namespace FashionHouse.Application.Features.Carts.Command
{
    public class CartAddItemCommandHandler : ICommandHandler<CartAddItemCommand, Domain.Entities.Cart>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public CartAddItemCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Domain.Entities.Cart> Handle(CartAddItemCommand command, CancellationToken cancellationToken)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByProductIdAsync(command.ProductId, cancellationToken);
            var available = inventory?.Quantity ?? 0;

            var cart = await _unitOfWork.CartRepository.GetOrCreateForCustomerAsync(command.CustomerId, cancellationToken);
            var existingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == command.ProductId);
            var requestedTotal = (existingItem?.Quantity ?? 0) + command.Quantity;

            if (requestedTotal > available)
                throw new InsufficientStockException($"Only {available} units available in stock.");

            if (existingItem is not null)
            {
                existingItem.Quantity = requestedTotal;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    Id = IdentityGenerator.NewSequentialGuid(),
                    CartId = cart.Id,
                    ProductId = command.ProductId,
                    Quantity = command.Quantity
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            // no EditAsync call here — cart is already tracked
            await _unitOfWork.SaveAsync(cancellationToken);

            return cart;
        }
    }
}