using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class AddressRepository : Repository<Address, Guid>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IList<Address>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.CustomerId == customerId,
                "IsDefault desc",
                null, 1, int.MaxValue, false, cancellationToken);

            return items;
        }

        public async Task<Address?> GetDefaultByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.CustomerId == customerId && x.IsDefault,
                null, null, 1, 1, false, cancellationToken);

            return items.FirstOrDefault();
        }

        public async Task ClearDefaultForCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.CustomerId == customerId && x.IsDefault,
                null, null, 1, int.MaxValue, false, cancellationToken);

            foreach (var address in items)
            {
                address.IsDefault = false;
                Edit(address);
            }
        }
    }
}