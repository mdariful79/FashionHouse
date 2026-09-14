using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.ProductImages.Query
{
    public class GetPrimaryImagesByProductIdsQuery : IQuery<IDictionary<Guid, ProductImage>>
    {
        public IEnumerable<Guid> ProductIds { get; set; } = new List<Guid>();
    }
}