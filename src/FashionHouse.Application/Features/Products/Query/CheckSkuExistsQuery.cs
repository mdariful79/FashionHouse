using System;
using System.Collections.Generic;
using System.Text;
using Cortex.Mediator.Queries;

namespace FashionHouse.Application.Features.Products.Query
{
    public class CheckSkuExistsQuery : IQuery<bool>
    {
        public string SKU { get; set; }
        public Guid? ExcludeId { get; set; }
    }
}
