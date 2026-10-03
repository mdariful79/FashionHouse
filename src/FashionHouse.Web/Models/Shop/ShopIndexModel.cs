using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Web.Models.Shop
{
    public class ShopIndexModel
    {
        public IList<Product> Products { get; set; } = new List<Product>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }

        public List<FilterItem> Categories { get; set; } = new();
        public List<FilterItem> SubCategories { get; set; } = new();  
        public List<ProductColor> AvailableColors { get; set; } = new();
        public List<ProductSize> AvailableSizes { get; set; } = new();

        public Guid? CategoryId { get; set; }
        public Guid? SubCategoryId { get; set; }
        public ProductColor? Color { get; set; }
        public ProductSize? Size { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Sort { get; set; }  
    }

    public class FilterItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }
}