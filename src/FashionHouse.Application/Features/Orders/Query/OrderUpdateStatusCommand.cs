using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Application.Features.Orders.Command
{
    public class OrderUpdateStatusCommand : ICommand<Order>
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}