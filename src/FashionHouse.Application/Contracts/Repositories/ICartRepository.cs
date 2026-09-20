using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface ICartRepository : IRepository<Cart, Guid>
    {
        Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
        Task<Cart> GetOrCreateForCustomerAsync(Guid customerId, CancellationToken cancellationToken);
    }
}