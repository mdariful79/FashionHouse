using FashionHouse.Application.Contracts;
using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Domain.Contracts;
using FashionHouse.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
namespace FashionHouse.Infrastructure.Data
{
    public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
    {
        public IProductRepository ProductRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public ISubCategoryRepository SubCategoryRepository { get; private set; }

        public ApplicationUnitOfWork(ApplicationDbContext dbContext, IProductRepository productRepository,
           ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository)
           : base(dbContext)
        {
            ProductRepository = productRepository;
            CategoryRepository = categoryRepository;
            SubCategoryRepository = subCategoryRepository;
        }
    }
}

