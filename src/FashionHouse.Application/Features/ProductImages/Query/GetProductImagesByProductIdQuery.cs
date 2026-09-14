using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.ProductImages.Query
{
    public class GetProductImagesByProductIdQuery : IQuery<IList<ProductImage>>
    {
        public Guid ProductId { get; set; }
    }
}