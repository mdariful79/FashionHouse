using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Application.Features.Products.Command
{
    public class ProductUpdateCommand : ICommand<Product>
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public List<ProductColor> Colors { get; set; } = new();
        public List<ProductSize> Sizes { get; set; } = new();
    }
}