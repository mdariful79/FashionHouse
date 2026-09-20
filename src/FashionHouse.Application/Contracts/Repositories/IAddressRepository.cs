using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface IAddressRepository : IRepository<Address, Guid>
    {
        Task<IList<Address>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
        Task<Address?> GetDefaultByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
        Task ClearDefaultForCustomerAsync(Guid customerId, CancellationToken cancellationToken);
    }
}