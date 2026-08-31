using Microsoft.AspNetCore.Mvc;

namespace FashionHouse.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
