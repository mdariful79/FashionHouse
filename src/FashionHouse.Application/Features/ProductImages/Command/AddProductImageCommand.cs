using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.ProductImages.Command
{
    public class AddProductImageCommand : ICommand<ProductImage>
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}