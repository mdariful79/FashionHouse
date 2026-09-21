using Cortex.Mediator;
using FashionHouse.Application.Features.Addresses.Command;
using FashionHouse.Application.Features.Addresses.Query;
using FashionHouse.Application.Features.Orders.Query;
using FashionHouse.Infrastructure.Identity;
using FashionHouse.Web.Models.AccountDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FashionHouse.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class AccountDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMediator _mediator;

        public AccountDashboardController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
        }


        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var orders = await _mediator.SendQueryAsync(new GetOrdersByCustomerIdQuery { CustomerId = user.Id }, cancellationToken);

            var model = new AccountDashboardModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                RecentOrders = orders.OrderByDescending(x => x.CreatedAt).Take(3).ToList()
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Keep the user signed in with the new security stamp
            await _signInManager.RefreshSignInAsync(user);

            TempData["StatusMessage"] = "Your password has been changed.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var model = new EditProfileModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth, // adjust property name once confirmed
                Email = user.Email!
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth; 

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // FirstName/LastName changes affect the "Hello, X" display via claims/cookie in some setups
            await _signInManager.RefreshSignInAsync(user);

            TempData["StatusMessage"] = "Your profile has been updated.";
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Addresses(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var addresses = await _mediator.SendQueryAsync(new GetAddressesByCustomerIdQuery { CustomerId = user.Id }, cancellationToken);

            return View(addresses);
        }

        [HttpGet]
        public IActionResult AddAddress()
        {
            return View(new AddressFormModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(AddressFormModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _mediator.SendCommandAsync(new AddressAddCommand
            {
                CustomerId = user.Id,
                FullName = model.FullName,
                Phone = model.Phone,
                FullAddress = model.FullAddress,
                District = model.District,
                PostalCode = model.PostalCode,
                IsDefault = model.IsDefault
            }, cancellationToken);

            TempData["StatusMessage"] = "Address added.";
            return RedirectToAction(nameof(Addresses));
        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var address = await _mediator.SendQueryAsync(new GetAddressByIdQuery { Id = id }, cancellationToken);

            if (address is null || address.CustomerId != user.Id)
                return NotFound();

            var model = new AddressFormModel
            {
                Id = address.Id,
                FullName = address.FullName,
                Phone = address.Phone,
                FullAddress = address.FullAddress,
                District = address.District,
                PostalCode = address.PostalCode,
                IsDefault = address.IsDefault
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(AddressFormModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            try
            {
                await _mediator.SendCommandAsync(new AddressUpdateCommand
                {
                    Id = model.Id,
                    CustomerId = user.Id,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    FullAddress = model.FullAddress,
                    District = model.District,
                    PostalCode = model.PostalCode,
                    IsDefault = model.IsDefault
                }, cancellationToken);

                TempData["StatusMessage"] = "Address updated.";
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Could not update this address.");
                return View(model);
            }

            return RedirectToAction(nameof(Addresses));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _mediator.SendCommandAsync(new AddressDeleteCommand { Id = id, CustomerId = user.Id }, cancellationToken);

            TempData["StatusMessage"] = "Address removed.";
            return RedirectToAction(nameof(Addresses));
        }
        public async Task<IActionResult> Orders(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var orders = await _mediator.SendQueryAsync(new GetOrdersByCustomerIdQuery { CustomerId = user.Id }, cancellationToken);

            return View(orders);
        }
    }
}