using FashionHouse.Application.Contracts.Repositories;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Infrastructure.Data.Repositories
{
    public class CustomerRepository : Repository<Customer, Guid>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}