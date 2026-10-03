using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class ProductRepository : Repository<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IList<Product>, int, int)> GetPagedProducts(GetAllProductsByPagingQuery query, CancellationToken cancellationToken)
        {
            var sortText = string.IsNullOrWhiteSpace(query.SortText) ? "CreatedAt desc" : query.SortText;

            return await GetDynamicAsync(
                x => query.SearchText == null || x.ProductName.Contains(query.SearchText),
                sortText,
                null,
                query.PageIndex,
                query.PageSize,
                true,
                cancellationToken);
        }

        public async Task<bool> IsDuplicateSku(string sku, Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
                return (await GetCountAsync(x => x.SKU == sku, cancellationToken)) > 0;
            else
                return (await GetCountAsync(x => x.SKU == sku && x.Id != id.Value, cancellationToken)) > 0;
        }

        public async Task<IList<Product>> GetActiveWithoutInventoryAsync(CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.IsActive && x.Inventory == null,
                "ProductName", null, 1, int.MaxValue, false, cancellationToken);

            return items;
        }

        public async Task<(IList<Product>, int, int)> GetActiveForShopAsync(
            GetActiveProductsForShopQuery query, CancellationToken ct)
        {
            var hasCategory = query.CategoryId.HasValue;
            var categoryId = query.CategoryId ?? Guid.Empty;

            var hasSubCategory = query.SubCategoryId.HasValue;
            var subCategoryId = query.SubCategoryId ?? Guid.Empty;

            // Category / sub-category are filtered in SQL.
            var (all, _, _) = await GetDynamicAsync(
                x => x.IsActive
                     && (!hasCategory || x.CategoryId == categoryId)
                     && (!hasSubCategory || x.SubCategoryId == subCategoryId),
                "CreatedAt desc",
                q => q.Include(p => p.ProductImages),
                1,
                int.MaxValue,
                true,
                ct);

            
            IEnumerable<Product> filtered = all;

            if (query.Color.HasValue)
            {
                var color = query.Color.Value;
                filtered = filtered.Where(p => p.Colors.Contains(color));
            }

            if (query.Size.HasValue)
            {
                var size = query.Size.Value;
                filtered = filtered.Where(p => p.Sizes.Contains(size));
            }

            
            if (query.MinPrice.HasValue)
            {
                var min = query.MinPrice.Value;
                filtered = filtered.Where(p => EffectivePrice(p) >= min);
            }

            if (query.MaxPrice.HasValue)
            {
                var max = query.MaxPrice.Value;
                filtered = filtered.Where(p => EffectivePrice(p) < max);
            }

            var list = query.SortBy switch
            {
                "price_asc" => filtered.OrderBy(EffectivePrice).ToList(),
                "price_desc" => filtered.OrderByDescending(EffectivePrice).ToList(),
                _ => filtered.ToList()   // already newest first
            };

            var total = list.Count;

            var page = list
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return (page, total, total);
        }

        private static decimal EffectivePrice(Product p)
            => p.DiscountedPrice.HasValue && p.DiscountedPrice < p.Price
                ? p.DiscountedPrice.Value
                : p.Price;

        public async Task<ShopFilterOptionsDto> GetShopFilterOptionsAsync(Guid? categoryId, CancellationToken ct)
        {
            var (products, _, _) = await GetDynamicAsync(
                x => x.IsActive,
                "ProductName",
                q => q.Include(p => p.Category).Include(p => p.SubCategory),
                1, int.MaxValue, true, ct);

            var categories = products
                .Where(p => p.Category != null)
                .GroupBy(p => new { p.Category!.Id, p.Category.Name })
                .Select(g => new ShopFilterItemDto { Id = g.Key.Id, Name = g.Key.Name, Count = g.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            var subSource = products.Where(p => p.SubCategory != null);
            if (categoryId.HasValue)
                subSource = subSource.Where(p => p.CategoryId == categoryId.Value);

            var subCategories = subSource
                .GroupBy(p => new { p.SubCategory!.Id, p.SubCategory.Name })
                .Select(g => new ShopFilterItemDto { Id = g.Key.Id, Name = g.Key.Name, Count = g.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new ShopFilterOptionsDto
            {
                Categories = categories,
                SubCategories = subCategories,
                Colors = products.SelectMany(p => p.Colors).Distinct().OrderBy(c => c).ToList(),
                Sizes = products.SelectMany(p => p.Sizes).Distinct().OrderBy(s => s).ToList()
            };
        }
        public async Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.Id == id,
                null,
                q => q.Include(p => p.ProductImages).Include(p => p.Category),
                1, 1, true, cancellationToken);

            return items.FirstOrDefault();
        }

        public async Task<IList<Product>> GetRelatedAsync(Guid categoryId, Guid excludeProductId, int take, CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.IsActive && x.CategoryId == categoryId && x.Id != excludeProductId,
                "ProductName",
                q => q.Include(p => p.ProductImages),
                1, take, true, cancellationToken);

            return items;
        }
    }
}