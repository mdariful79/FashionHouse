using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Domain.Entities
{
    public class CartItem : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        // Navigation
        public Cart Cart { get; set; }
        public Product Product { get; set; }
        public ProductColor Color { get; set; }
        public ProductSize Size { get; set; }
    }
}