using FashionHouse.Infrastructure.Data;
using FashionHouse.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionHouse.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var model = new HomeModel();

            model.RecentOrders = await _db.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)          // your order date property
                .Take(6)
                .Select(o => new RecentOrderRow
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.Customer.FirstName +' '+o.Customer.LastName,        // your customer name property
                    OrderedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Sum(i => i.Quantity),
                    PaymentMethod = o.PaymentMethod,
                    Total = o.GrandTotal,
                    Status = o.Status.ToString()
                })
                .ToListAsync(ct);

            return View(model);
        }
    }
}