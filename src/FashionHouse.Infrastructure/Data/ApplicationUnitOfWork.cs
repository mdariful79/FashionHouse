using FashionHouse.Application.Contracts;
using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Domain.Contracts;
using FashionHouse.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
namespace FashionHouse.Infrastructure.Data
{
    public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; private set; }
        public ISubCategoryRepository SubCategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductImageRepository ProductImageRepository { get; private set; }
        public IInventoryRepository InventoryRepository { get; private set; }
        public ICustomerRepository CustomerRepository { get; private set; }
        public IAddressRepository AddressRepository { get; private set; }
        public ICartRepository CartRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }

        public ApplicationUnitOfWork(ApplicationDbContext dbContext, IProductRepository productRepository,
           ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository,
           IProductImageRepository productImageRepository, IInventoryRepository inventoryRepository,
           ICustomerRepository customerRepository, IAddressRepository addressRepository,
           ICartRepository cartRepository, IOrderRepository orderRepository)
           : base(dbContext)
        {
            CategoryRepository = categoryRepository;
            SubCategoryRepository = subCategoryRepository;
            ProductRepository = productRepository;
            ProductImageRepository = productImageRepository;
            InventoryRepository = inventoryRepository;
            CustomerRepository = customerRepository;
            AddressRepository = addressRepository;
            CartRepository = cartRepository;
            OrderRepository = orderRepository;
        }
    }
}

