using Cortex.Mediator;
using FashionHouse.Application.Features.Carts.Query;
using FashionHouse.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FashionHouse.Web.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartSummaryViewComponent(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!(User.Identity?.IsAuthenticated ?? false) || !User.IsInRole("Customer"))
                return View(new CartSummaryModel { ItemCount = 0, Total = 0 });

            var userId = _userManager.GetUserId(HttpContext.User);
            var cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery { CustomerId = Guid.Parse(userId!) },
                HttpContext.RequestAborted);

            var total = cart.CartItems.Sum(x => (x.Product.DiscountedPrice ?? x.Product.Price) * x.Quantity);

            return View(new CartSummaryModel
            {
                ItemCount = cart.CartItems.Sum(x => x.Quantity),
                Total = total
            });
        }
    }

    public class CartSummaryModel
    {
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
    }
}