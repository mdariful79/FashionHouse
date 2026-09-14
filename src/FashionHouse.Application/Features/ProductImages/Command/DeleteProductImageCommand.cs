using Cortex.Mediator.Commands;

namespace FashionHouse.Application.Features.ProductImages.Command
{
    public class DeleteProductImageCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}