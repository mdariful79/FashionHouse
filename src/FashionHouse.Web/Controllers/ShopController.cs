using Cortex.Mediator;
using FashionHouse.Application.Features.Products.Query;
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

        public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
        {
            const int pageSize = 12;

            var (products, total, _) = await _mediator.SendQueryAsync(new GetActiveProductsForShopQuery
            {
                PageIndex = page,
                PageSize = pageSize
            }, cancellationToken);

            var model = new ShopIndexModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                TotalItems = total,
                PageSize = pageSize
            };

            return View(model);
        }

        public IActionResult ShopDetails(Guid id)
        {
            // TODO: load the single product by id (next step)
            return View();
        }
    }
}