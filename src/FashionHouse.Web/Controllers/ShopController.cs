using Microsoft.AspNetCore.Mvc;

namespace FashionHouse.Web.Controllers
{
    public class ShopController : Controller
    {
        // GET: /Shop
        public IActionResult Index()
        {
            // TODO: replace static placeholder products below with real data
            // via IProductRepository / a Products query once product listing
            // (with paging/filtering) is wired up.
            return View();
        }
        public IActionResult ShopDetails()
        {
            // TODO: replace static placeholder products below with real data
            // via IProductRepository / a Products query once product listing
            // (with paging/filtering) is wired up.
            return View();
        }
    }
}
