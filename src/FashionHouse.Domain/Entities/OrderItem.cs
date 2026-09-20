using FashionHouse.Domain.Contracts;

namespace FashionHouse.Domain.Entities
{
    public class OrderItem : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }

        // Navigation
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}