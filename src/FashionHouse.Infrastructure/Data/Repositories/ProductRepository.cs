using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class ProductRepository : Repository<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
