using FashionHouse.Domain.Entities;

namespace FashionHouse.Web.Models.Home
{
    public class HomeIndexModel
    {
        public IList<Product> BestSellers { get; set; } = new List<Product>();
        public IList<Product> NewArrivals { get; set; } = new List<Product>();
        public IList<Product> HotSales { get; set; } = new List<Product>();
    }
}