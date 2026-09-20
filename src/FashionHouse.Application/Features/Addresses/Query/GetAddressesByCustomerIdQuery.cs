using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Addresses.Query
{
    public class GetAddressesByCustomerIdQuery : IQuery<IList<Address>>
    {
        public Guid CustomerId { get; set; }
    }
}