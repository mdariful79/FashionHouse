using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entites;

namespace FashionHouse.Domain.Entities;

public class Category : IAggregateRoot<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}