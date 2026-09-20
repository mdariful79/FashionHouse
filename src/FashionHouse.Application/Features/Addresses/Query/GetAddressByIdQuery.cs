using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Addresses.Query
{
    public class GetAddressByIdQuery : IQuery<Address?>
    {
        public Guid Id { get; set; }
    }
}