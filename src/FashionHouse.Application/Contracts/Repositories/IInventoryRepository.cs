using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface IInventoryRepository : IRepository<Inventory, Guid>
    {
        Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);

        Task<(IList<Inventory>, int, int)> GetPagedInventoriesAsync(
            Features.Inventories.Query.GetAllInventoriesByPagingQuery query,
            CancellationToken cancellationToken);
    }
}
