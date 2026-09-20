using Cortex.Mediator;
using FashionHouse.Infrastructure.Extensions;
using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Carts.Command;
using FashionHouse.Application.Features.Carts.Query;
using FashionHouse.Infrastructure.Identity;
using FashionHouse.Web.Areas.Admin.Models;
using FashionHouse.Web.Codes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FashionHouse.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(IMediator mediator, UserManager<ApplicationUser> userManager, ILogger<CartController> logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _logger = logger;
        }

        private Guid CustomerId => Guid.Parse(_userManager.GetUserId(User)!);

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery { CustomerId = CustomerId }, cancellationToken);
            return View(cart);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1, CancellationToken cancellationToken = default)
        {
            try
            {
                await _mediator.SendCommandAsync(new CartAddItemCommand
                {
                    CustomerId = CustomerId,
                    ProductId = productId,
                    Quantity = quantity
                }, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Added to your cart.", Type = ResponseTypes.Success });
            }
            catch (InsufficientStockException iex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = iex.Message, Type = ResponseTypes.Danger });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add item to cart");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to add item to cart.", Type = ResponseTypes.Danger });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateQuantity(Guid cartItemId, int quantity, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new CartUpdateItemQuantityCommand
                {
                    CustomerId = CustomerId,
                    CartItemId = cartItemId,
                    Quantity = quantity
                }, cancellationToken);

                var cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery { CustomerId = CustomerId }, cancellationToken);
                var total = cart.CartItems.Sum(x => (x.Product.DiscountedPrice ?? x.Product.Price) * x.Quantity);
                var line = cart.CartItems.FirstOrDefault(x => x.Id == cartItemId);
                var lineTotal = line is null ? 0 : (line.Product.DiscountedPrice ?? line.Product.Price) * line.Quantity;

                return Json(new { success = true, lineTotal, subTotal = total, itemCount = cart.CartItems.Sum(x => x.Quantity) });
            }
            catch (InsufficientStockException iex)
            {
                return Json(new { success = false, message = iex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cart quantity");
                return Json(new { success = false, message = "Failed to update quantity." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> RemoveItem(Guid cartItemId, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new CartRemoveItemCommand
                {
                    CustomerId = CustomerId,
                    CartItemId = cartItemId
                }, cancellationToken);

                var cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery { CustomerId = CustomerId }, cancellationToken);
                var total = cart.CartItems.Sum(x => (x.Product.DiscountedPrice ?? x.Product.Price) * x.Quantity);

                return Json(new { success = true, subTotal = total, itemCount = cart.CartItems.Sum(x => x.Quantity) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove cart item");
                return Json(new { success = false, message = "Failed to remove item." });
            }
        }
    }
}