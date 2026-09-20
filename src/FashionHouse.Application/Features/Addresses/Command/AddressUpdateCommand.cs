using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Addresses.Command
{
    public class AddressUpdateCommand : ICommand<Address>
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public bool IsDefault { get; set; }
    }
}