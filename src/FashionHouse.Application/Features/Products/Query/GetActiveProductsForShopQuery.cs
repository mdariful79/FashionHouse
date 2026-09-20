using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetActiveProductsForShopQuery : IQuery<(IList<Product>, int, int)>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public string? SearchText { get; set; }
        public Guid? CategoryId { get; set; }
    }
}