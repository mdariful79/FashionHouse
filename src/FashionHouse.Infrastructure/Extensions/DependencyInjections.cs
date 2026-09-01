using FashionHouse.Application.Contracts;
using FashionHouse.Infrastructure.Data;
using FashionHouse.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Infrastructure.Extensions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructureDependency(this IServiceCollection services)
        {
            services.AddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }
    }
}
 
