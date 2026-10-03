using Cortex.Mediator.Queries;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Application.Features.Products.Query
{
    public class GetActiveProductsForShopQuery : IQuery<(IList<Product>, int, int)>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public string? SearchText { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SubCategoryId { get; set; }
        public ProductColor? Color { get; set; }
        public ProductSize? Size { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }   
    }
}