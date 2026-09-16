using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class ProductRepository : Repository<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IList<Product>, int, int)> GetPagedProducts(GetAllProductsByPagingQuery query,
            CancellationToken cancellationToken)
        {
            return await GetDynamicAsync(
                x => query.SearchText == null || x.ProductName.Contains(query.SearchText),
                query.SortText,
                null,
                query.PageIndex,
                query.PageSize,
                true,
                cancellationToken);
        }

        //public async Task<bool> IsDuplicateProductName(string productName, Guid? id, CancellationToken cancellationToken)
        //{
        //    if (!id.HasValue)
        //        return (await GetCountAsync(x => x.ProductName == productName, cancellationToken)) > 0;
        //    else
        //        return (await GetCountAsync(x => x.ProductName == productName && x.Id != id.Value, cancellationToken)) > 0;
        //}

        public async Task<bool> IsDuplicateSku(string sku, Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
                return (await GetCountAsync(x => x.SKU == sku, cancellationToken)) > 0;
            else
                return (await GetCountAsync(x => x.SKU == sku && x.Id != id.Value, cancellationToken)) > 0;
        }
    }
}