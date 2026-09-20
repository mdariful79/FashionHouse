using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Carts.Query
{
    public class GetCartByCustomerIdQuery : IQuery<Domain.Entities.Cart>
    {
        public Guid CustomerId { get; set; }
    }
}