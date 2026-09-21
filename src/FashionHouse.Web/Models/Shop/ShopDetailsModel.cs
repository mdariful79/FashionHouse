using FashionHouse.Domain.Entities;

namespace FashionHouse.Web.Models.Shop
{
    public class ShopDetailsModel
    {
        public Product Product { get; set; } = null!;
        public int AvailableStock { get; set; }
        public IList<Product> RelatedProducts { get; set; } = new List<Product>();
    }
}