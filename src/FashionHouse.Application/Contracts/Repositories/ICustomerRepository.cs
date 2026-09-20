using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface ICustomerRepository : IRepository<Customer, Guid>
    {
    }
}