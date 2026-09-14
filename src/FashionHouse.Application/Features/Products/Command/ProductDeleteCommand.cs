using Cortex.Mediator.Commands;

namespace FashionHouse.Application.Features.Products.Command
{
    public class ProductDeleteCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}