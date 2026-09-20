using FashionHouse.Domain.Contracts;

namespace FashionHouse.Domain.Entities
{
    public class Cart : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Customer Customer { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}