using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class ProductImageRepository : Repository<ProductImage, Guid>, IProductImageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductImageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            return await _dbContext.ProductImages
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<IDictionary<Guid, ProductImage>> GetPrimaryImagesByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken)
        {
            var images = await _dbContext.ProductImages
                .Where(x => productIds.Contains(x.ProductId) && x.IsPrimary)
                .ToListAsync(cancellationToken);

            return images.ToDictionary(x => x.ProductId, x => x);
        }

        public async Task ClearPrimaryFlagAsync(Guid productId, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.ProductImages
                .Where(x => x.ProductId == productId && x.IsPrimary)
                .ToListAsync(cancellationToken);

            foreach (var img in existing)
                img.IsPrimary = false;
        }
    }
}