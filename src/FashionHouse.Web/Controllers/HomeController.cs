using Cortex.Mediator;
using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Web.Models;
using FashionHouse.Web.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FashionHouse.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var data = await _mediator.SendQueryAsync(new GetHomeProductsQuery { Take = 8 }, cancellationToken);

            var model = new HomeIndexModel
            {
                BestSellers = data.BestSellers,
                NewArrivals = data.NewArrivals,
                HotSales = data.HotSales
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}