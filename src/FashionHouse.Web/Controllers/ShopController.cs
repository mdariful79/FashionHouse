using Cortex.Mediator;
using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Enums;
using FashionHouse.Web.Models.Shop;
using Microsoft.AspNetCore.Mvc;

namespace FashionHouse.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly IMediator _mediator;

        public ShopController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(
            Guid? categoryId,
            Guid? subCategoryId,
            ProductColor? color,
            ProductSize? size,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            const int pageSize = 12;
            if (page < 1) page = 1;

            // Products (filtered + paged)
            var (products, total, _) = await _mediator.SendQueryAsync(new GetActiveProductsForShopQuery
            {
                PageIndex = page,
                PageSize = pageSize,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                Color = color,
                Size = size,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sort
            }, cancellationToken);

            // Sidebar data (categories, brands/sub-categories, colors, sizes)
            var filters = await _mediator.SendQueryAsync(new GetShopFilterOptionsQuery
            {
                CategoryId = categoryId
            }, cancellationToken);

            var model = new ShopIndexModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                TotalItems = total,
                PageSize = pageSize,

                Categories = filters.Categories
                    .Select(c => new FilterItem { Id = c.Id, Name = c.Name, Count = c.Count })
                    .ToList(),
                SubCategories = filters.SubCategories
                    .Select(s => new FilterItem { Id = s.Id, Name = s.Name, Count = s.Count })
                    .ToList(),
                AvailableColors = filters.Colors,
                AvailableSizes = filters.Sizes,

                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                Color = color,
                Size = size,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Sort = sort
            };

            return View(model);
        }

        public async Task<IActionResult> ShopDetails(Guid id, CancellationToken cancellationToken)
        {
            var product = await _mediator.SendQueryAsync(new GetProductDetailsForShopQuery { Id = id }, cancellationToken);

            if (product is null || !product.IsActive)
                return NotFound();

            var inventory = await _mediator.SendQueryAsync(new FashionHouse.Application.Features.Inventories.Query.GetInventoryByProductIdQuery
            {
                ProductId = id
            }, cancellationToken);

            var related = await _mediator.SendQueryAsync(new GetRelatedProductsQuery
            {
                CategoryId = product.CategoryId,
                ExcludeProductId = product.Id,
                Take = 4
            }, cancellationToken);

            var model = new ShopDetailsModel
            {
                Product = product,
                AvailableStock = inventory?.Quantity ?? 0,
                RelatedProducts = related
            };

            return View(model);
        }
    }
}