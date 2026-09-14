using Cortex.Mediator.Commands;

namespace FashionHouse.Application.Features.ProductImages.Command
{
    public class SetPrimaryProductImageCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
    }
}