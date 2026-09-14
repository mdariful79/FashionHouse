using FashionHouse.Domain.Contracts;

namespace FashionHouse.Domain.Entities
{
    public class ProductImage : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; } = false;

        //Navigation
        public Product Product { get; set; }
    }
}