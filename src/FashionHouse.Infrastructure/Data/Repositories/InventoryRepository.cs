using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Application.Features.Inventories.Query;
using FashionHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class InventoryRepository : Repository<Inventory, Guid>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.ProductId == productId,
                null, null, 1, 1, true, cancellationToken);

            return items.FirstOrDefault();
        }

        public async Task<(IList<Inventory>, int, int)> GetPagedInventoriesAsync(
            GetAllInventoriesByPagingQuery query, CancellationToken cancellationToken)
        {
            return await GetDynamicAsync(
                x => (!query.LowStockOnly || x.Quantity <= x.ReorderLevel)
                     && (string.IsNullOrEmpty(query.SearchText) || x.Product.ProductName.Contains(query.SearchText)
                         || x.Product.SKU.Contains(query.SearchText)),
                query.SortText,
                q => q.Include(i => i.Product),
                query.PageIndex,
                query.PageSize,
                true,
                cancellationToken);
        }
    }
}