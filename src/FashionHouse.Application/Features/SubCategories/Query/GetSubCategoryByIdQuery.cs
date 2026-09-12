using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.SubCategories.Query
{
    public class GetSubCategoryByIdQuery : IQuery<SubCategory?>
    {
        public Guid Id { get; set; }
    }
}