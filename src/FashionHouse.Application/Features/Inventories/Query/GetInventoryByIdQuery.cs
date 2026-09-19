using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Features.Inventories.Query
{
    public class GetInventoryByIdQuery : IQuery<Inventory?>
    {
        public Guid Id { get; set; }
    }
}
