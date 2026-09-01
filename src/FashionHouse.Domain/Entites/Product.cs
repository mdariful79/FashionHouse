using FashionHouse.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Domain.Entites
{
    public class Product : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
    }
}
