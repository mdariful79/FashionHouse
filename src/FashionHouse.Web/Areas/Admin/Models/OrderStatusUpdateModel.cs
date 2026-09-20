using FashionHouse.Domain.Enums;

namespace FashionHouse.Web.Areas.Admin.Models
{
    public class OrderStatusUpdateModel
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}