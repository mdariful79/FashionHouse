using Cortex.Mediator.Commands;

namespace FashionHouse.Application.Features.SubCategories.Command
{
    public class SubCategoryDeleteCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}