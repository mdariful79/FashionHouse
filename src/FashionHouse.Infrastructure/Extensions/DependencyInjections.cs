using FashionHouse.Application.Contracts;
using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Application.Contracts.Services;
using FashionHouse.Infrastructure.Data;
using FashionHouse.Infrastructure.Data.Repositories;
using FashionHouse.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Infrastructure.Extensions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructureDependency(this IServiceCollection services, string rootPath)
        {
            services.AddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IFileStorageService, FileStorageService>(x => new FileStorageService(rootPath));
            services.AddSingleton<IServerTime, ServerTime>();
            services.AddScoped<IEmailService, EmailService>();
          

            return services;
        }
    }
}
 
