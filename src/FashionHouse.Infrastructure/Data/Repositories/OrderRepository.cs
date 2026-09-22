using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Application.Features.Orders.Query;
using FashionHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class OrderRepository : Repository<Order, Guid>, IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var (items, _, _) = await GetDynamicAsync(
                x => x.CustomerId == customerId,
                "CreatedAt desc",
                q => q.Include(o => o.OrderItems),
                1, int.MaxValue, true, cancellationToken);

            return items;
        }

        public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken)
        {
            return await GetCountAsync(x => x.OrderNumber == orderNumber, cancellationToken) > 0;
        }
        public async Task<(IList<Order>, int, int)> GetPagedOrdersAsync(GetAllOrdersByPagingQuery query, CancellationToken cancellationToken)
        {
            return await GetDynamicAsync(
                x => (!query.StatusFilter.HasValue || x.Status == query.StatusFilter.Value)
                     && (string.IsNullOrEmpty(query.SearchText)
                         || x.OrderNumber.Contains(query.SearchText)
                         || x.ShippingFullName.Contains(query.SearchText)
                         || x.ShippingPhone.Contains(query.SearchText)),
               string.IsNullOrWhiteSpace(query.SortText) ? "CreatedAt desc" : query.SortText,
                //query.SortText,
                q => q.Include(o => o.OrderItems),
                query.PageIndex,
                query.PageSize,
                true,
                cancellationToken);
        }
    }
}