using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetRelatedProductsQuery : IQuery<IList<Product>>
    {
        public Guid CategoryId { get; set; }
        public Guid ExcludeProductId { get; set; }
        public int Take { get; set; } = 4;
    }
}