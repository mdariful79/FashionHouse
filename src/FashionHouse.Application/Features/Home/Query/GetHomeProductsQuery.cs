using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetHomeProductsQuery : IQuery<HomeProductsDto>
    {
        public int Take { get; set; } = 8;
    }

    public class HomeProductsDto
    {
        public IList<Product> BestSellers { get; set; } = new List<Product>();
        public IList<Product> NewArrivals { get; set; } = new List<Product>();
        public IList<Product> HotSales { get; set; } = new List<Product>();
    }
}