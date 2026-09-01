using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Contracts
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
    }
}
