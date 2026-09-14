using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface IProductImageRepository : IRepository<ProductImage, Guid>
    {
        Task<IList<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
        Task<IDictionary<Guid, ProductImage>> GetPrimaryImagesByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken);
        Task ClearPrimaryFlagAsync(Guid productId, CancellationToken cancellationToken);
    }
}