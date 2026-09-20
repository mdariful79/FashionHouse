using Cortex.Mediator.Commands;

namespace FashionHouse.Application.Features.Addresses.Command
{
    public class AddressDeleteCommand : ICommand<bool>
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
    }
}