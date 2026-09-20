using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Application.Features.Orders.Command
{
    public class OrderUpdateStatusCommandHandler : ICommandHandler<OrderUpdateStatusCommand, Order>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public OrderUpdateStatusCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> Handle(OrderUpdateStatusCommand command, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(command.Id, cancellationToken);

            if (order is null)
                throw new Exception("Order not found.");

            var wasCancelledOrReturned = order.Status is OrderStatus.Cancelled or OrderStatus.Returned;
            var isNowCancelledOrReturned = command.Status is OrderStatus.Cancelled or OrderStatus.Returned;

            // Restock items only on the transition INTO Cancelled/Returned, not every save
            if (!wasCancelledOrReturned && isNowCancelledOrReturned)
            {
                foreach (var item in order.OrderItems)
                {
                    var inventory = await _unitOfWork.InventoryRepository.GetByProductIdAsync(item.ProductId, cancellationToken);

                    if (inventory is not null)
                    {
                        inventory.Quantity += item.Quantity;
                        inventory.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.InventoryRepository.EditAsync(inventory, cancellationToken);
                    }
                }
            }

            order.Status = command.Status;
            order.PaymentStatus = command.PaymentStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.OrderRepository.EditAsync(order, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return order;
        }
    }
}