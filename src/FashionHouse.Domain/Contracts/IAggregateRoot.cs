using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Domain.Contracts
{
    public interface IAggregateRoot<TKey>
    {
        TKey Id { get; set; }
    }
}
