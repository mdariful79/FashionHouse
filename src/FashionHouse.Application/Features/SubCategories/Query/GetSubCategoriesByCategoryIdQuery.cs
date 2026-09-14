using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.SubCategories.Query
{
    public class GetSubCategoriesByCategoryIdQuery : IQuery<IList<SubCategory>>
    {
        public Guid CategoryId { get; set; }
    }
}