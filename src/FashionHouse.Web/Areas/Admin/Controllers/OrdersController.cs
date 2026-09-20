using Cortex.Mediator;
using FashionHouse.Application.Features.Orders.Command;
using FashionHouse.Application.Features.Orders.Query;
using FashionHouse.Domain.Entities;
using FashionHouse.Infrastructure.Extensions;
using FashionHouse.Web.Areas.Admin.Models;
using FashionHouse.Web.Codes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace FashionHouse.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IMediator _mediator;

        public OrdersController(ILogger<OrdersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<JsonResult> GetPagedOrders([FromBody] OrderListModel model, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllOrdersByPagingQuery
                {
                    PageIndex = model.Start / model.Length + 1,
                    PageSize = model.Length,
                    SearchText = model.Search.Value,
                    SortText = model.FormatSortExpression("OrderNumber", "ShippingFullName", "GrandTotal", "Status", "CreatedAt"),
                    StatusFilter = model.StatusFilter
                };

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync<GetAllOrdersByPagingQuery,
                    (IList<Order>, int, int)>(query, cancellationToken);

                var result = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                                HttpUtility.HtmlEncode(item.OrderNumber),
                                HttpUtility.HtmlEncode(item.ShippingFullName),
                                item.OrderItems.Sum(x => x.Quantity).ToString(),
                                item.GrandTotal.ToString("N0"),
                                item.Status.ToString(),
                                item.PaymentStatus.ToString(),
                                item.CreatedAt.ToString("dd MMM yyyy, hh:mm tt"),
                                item.Id.ToString()
                            }).ToArray()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order list");
                return Json(FashionHouse.Domain.Utilities.DataTables.EmptyResult);
            }
        }

        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var order = await _mediator.SendQueryAsync(new GetOrderByIdQuery { Id = id }, cancellationToken);

            if (order is null)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Order not found.", Type = ResponseTypes.Danger });

                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(OrderStatusUpdateModel model, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new OrderUpdateStatusCommand
                {
                    Id = model.Id,
                    Status = model.Status,
                    PaymentStatus = model.PaymentStatus
                }, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Order status updated.", Type = ResponseTypes.Success });
            }
            catch (Exception ex)
            {
                const string errorMessage = "Failed to update order status.";
                _logger.LogError(ex, errorMessage);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
    }
}