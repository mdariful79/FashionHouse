using FashionHouse.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Domain.Entities
{
    public class Inventory : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; } = 5;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //Navigation
        public Product Product { get; set; }

    }
}
