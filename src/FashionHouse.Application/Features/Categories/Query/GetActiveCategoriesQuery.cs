using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Categories.Query
{
    public class GetActiveCategoriesQuery : IQuery<IList<Category>>
    {
    }
}