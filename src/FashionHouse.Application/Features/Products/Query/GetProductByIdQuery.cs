using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetProductByIdQuery : IQuery<Product?>
    {
        public Guid Id { get; set; }
    }
}