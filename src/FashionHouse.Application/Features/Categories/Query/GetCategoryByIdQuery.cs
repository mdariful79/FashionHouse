using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Categories.Query
{
    public class GetCategoryByIdQuery : IQuery<Category?>
    {
        public Guid Id { get; set; }
    }
}