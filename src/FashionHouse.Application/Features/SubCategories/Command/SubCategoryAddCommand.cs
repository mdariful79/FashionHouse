using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.SubCategories.Command
{
    public class SubCategoryAddCommand : ICommand<SubCategory>
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}