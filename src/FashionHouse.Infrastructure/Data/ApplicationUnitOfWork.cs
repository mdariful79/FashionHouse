using FashionHouse.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
namespace FashionHouse.Infrastructure.Data
{
    public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
    {
       
        //public IProductRepository ProductRepository { get; private set; }
      

        public ApplicationUnitOfWork(
            ApplicationDbContext context
            
            //IProductRepository productRepository
            ) : base(context)
        {
        
            //ProductRepository = productRepository;
      

        }
    }
}

