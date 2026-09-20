using FashionHouse.Application.Features.Orders.Query;
using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<IList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
        Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken);
        Task<(IList<Order>, int, int)> GetPagedOrdersAsync(GetAllOrdersByPagingQuery query, CancellationToken cancellationToken);
    }
}