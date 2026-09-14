using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Contracts
{
    public interface IApplicationUnitOfWork : IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public ISubCategoryRepository SubCategoryRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IProductImageRepository ProductImageRepository { get; }

    }
}
