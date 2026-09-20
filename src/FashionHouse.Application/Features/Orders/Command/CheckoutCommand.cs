using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Orders.Command
{
    public class CheckoutCommand : ICommand<Order>
    {
        public Guid CustomerId { get; set; }
        public Guid AddressId { get; set; }
        public string PaymentMethod { get; set; } = "Cash on Delivery";
        public string? Notes { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
    }
}