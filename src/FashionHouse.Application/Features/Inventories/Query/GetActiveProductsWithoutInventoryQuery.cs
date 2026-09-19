using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetActiveProductsWithoutInventoryQuery : IQuery<IList<Product>>
    {
    }
}