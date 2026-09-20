using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class CartRepository : Repository<Cart, Guid>, ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await _dbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
        }

        public async Task<Cart> GetOrCreateForCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var cart = await GetByCustomerIdAsync(customerId, cancellationToken);

            if (cart is not null)
                return cart;

            cart = new Cart
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                CustomerId = customerId,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Carts.AddAsync(cart, cancellationToken);
         
            return cart;
        }
    }
}