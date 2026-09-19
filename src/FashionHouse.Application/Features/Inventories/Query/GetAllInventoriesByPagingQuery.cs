using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Features.Inventories.Query
{
    public class GetAllInventoriesByPagingQuery : IQuery<(IList<Inventory>, int, int)>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchText { get; set; }
        public string SortText { get; set; }
        public bool LowStockOnly { get; set; }
    }
}
