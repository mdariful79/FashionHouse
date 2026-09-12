using FashionHouse.Domain.Contracts;
namespace FashionHouse.Domain.Entities;

public class SubCategory : IAggregateRoot<Guid>
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }     
    public string Name { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

}