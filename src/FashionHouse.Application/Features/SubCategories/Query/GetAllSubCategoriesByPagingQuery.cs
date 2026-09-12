using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.SubCategories.Query
{
    public class GetAllSubCategoriesByPagingQuery : IQuery<(IList<SubCategory>, int, int)>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchText { get; set; }
        public string SortText { get; set; }
    }
}