using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Features.Inventories.Command
{
    public class InventoryAdjustStockCommand : ICommand<Inventory>
    {
        public Guid ProductId { get; set; }
        public int ChangeQuantity { get; set; }   // positive = add stock, negative = remove stock
        public int? ReorderLevel { get; set; }    // update reorder threshold at the same time
    }
}