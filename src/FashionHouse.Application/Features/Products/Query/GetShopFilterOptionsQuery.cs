using Cortex.Mediator.Queries;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetShopFilterOptionsQuery : IQuery<ShopFilterOptionsDto>
    {
        public Guid? CategoryId { get; set; }
    }

    public class ShopFilterItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }

    public class ShopFilterOptionsDto
    {
        public List<ShopFilterItemDto> Categories { get; set; } = new();
        public List<ShopFilterItemDto> SubCategories { get; set; } = new();
        public List<ProductColor> Colors { get; set; } = new();
        public List<ProductSize> Sizes { get; set; } = new();
    }
}