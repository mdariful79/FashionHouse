using FashionHouse.Domain.Entities;

namespace FashionHouse.Web.Models.Shop
{
    public class ShopIndexModel
    {
        public IList<Product> Products { get; set; } = new List<Product>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public IList<Category> Categories { get; set; } = new List<Category>();
    }
}