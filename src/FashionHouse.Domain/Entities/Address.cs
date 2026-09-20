using FashionHouse.Domain.Contracts;

namespace FashionHouse.Domain.Entities
{
    public class Address : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public bool IsDefault { get; set; }

        // Navigation
        public Customer Customer { get; set; }
    }
}