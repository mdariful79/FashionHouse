using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Application.Features.Orders.Query
{
    public class GetAllOrdersByPagingQuery : IQuery<(IList<Order>, int, int)>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchText { get; set; }
        public string SortText { get; set; }
        public OrderStatus? StatusFilter { get; set; }
    }
}