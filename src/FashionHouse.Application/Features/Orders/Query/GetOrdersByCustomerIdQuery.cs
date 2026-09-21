using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Orders.Query
{
    public class GetOrdersByCustomerIdQuery : IQuery<IList<Order>>
    {
        public Guid CustomerId { get; set; }
    }
}