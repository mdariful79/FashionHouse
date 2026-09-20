using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Orders.Query
{
    public class GetOrderByIdQuery : IQuery<Order?>
    {
        public Guid Id { get; set; }
    }
}