using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        //Task<bool> IsDuplicateProductName(string productName, Guid? id, CancellationToken cancellationToken);

        Task<bool> IsDuplicateSku(string sku, Guid? id, CancellationToken cancellationToken);
        Task<IList<Product>> GetActiveWithoutInventoryAsync(CancellationToken cancellationToken);
        Task<(IList<Product>, int, int)> GetActiveForShopAsync(GetActiveProductsForShopQuery query, CancellationToken cancellationToken);

        Task<(IList<Product>, int, int)> GetPagedProducts(
            Features.Products.Query.GetAllProductsByPagingQuery query,
            CancellationToken cancellationToken);
    }
}