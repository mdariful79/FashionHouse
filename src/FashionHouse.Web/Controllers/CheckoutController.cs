using Cortex.Mediator;
using FashionHouse.Application.Exceptions;
using FashionHouse.Infrastructure.Extensions;
using FashionHouse.Application.Features.Addresses.Command;
using FashionHouse.Application.Features.Addresses.Query;
using FashionHouse.Application.Features.Carts.Query;
using FashionHouse.Application.Features.Orders.Command;
using FashionHouse.Infrastructure.Identity;
using FashionHouse.Web.Areas.Admin.Models;
using FashionHouse.Web.Codes;
using FashionHouse.Web.Models.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FashionHouse.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CheckoutController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IMediator mediator, UserManager<ApplicationUser> userManager, ILogger<CheckoutController> logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _logger = logger;
        }

        private Guid CustomerId => Guid.Parse(_userManager.GetUserId(User)!);

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var cart = await _mediator.SendQueryAsync(new GetCartByCustomerIdQuery { CustomerId = CustomerId }, cancellationToken);

            if (!cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            var addresses = await _mediator.SendQueryAsync(new GetAddressesByCustomerIdQuery { CustomerId = CustomerId }, cancellationToken);

            var model = new CheckoutModel
            {
                Cart = cart,
                Addresses = addresses,
                SelectedAddressId = addresses.FirstOrDefault(x => x.IsDefault)?.Id ?? addresses.FirstOrDefault()?.Id ?? Guid.Empty
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(NewAddressModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                await _mediator.SendCommandAsync(new AddressAddCommand
                {
                    CustomerId = CustomerId,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    FullAddress = model.FullAddress,
                    District = model.District,
                    PostalCode = model.PostalCode,
                    IsDefault = model.IsDefault
                }, cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(Guid addressId, string paymentMethod, string? notes, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _mediator.SendCommandAsync(new CheckoutCommand
                {
                    CustomerId = CustomerId,
                    AddressId = addressId,
                    PaymentMethod = string.IsNullOrEmpty(paymentMethod) ? "Cash on Delivery" : paymentMethod,
                    Notes = notes,
                    ShippingFee = 0,
                    Discount = 0
                }, cancellationToken);

                return RedirectToAction(nameof(Confirmation), new { orderNumber = order.OrderNumber });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = ex.Message, Type = ResponseTypes.Danger });

                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Confirmation(string orderNumber)
        {
            ViewBag.OrderNumber = orderNumber;
            return View();
        }
    }
}