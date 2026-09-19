using Cortex.Mediator;
using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Inventories.Command;
using FashionHouse.Application.Features.Inventories.Query;
using FashionHouse.Application.Features.Products.Query;
using FashionHouse.Domain.Entities;
using FashionHouse.Infrastructure.Extensions;
using FashionHouse.Web.Areas.Admin.Models;
using FashionHouse.Web.Codes;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace FashionHouse.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class StocksController : Controller
    {
        private readonly ILogger<StocksController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public StocksController(ILogger<StocksController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index() => View();
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var products = await _mediator.SendQueryAsync<GetActiveProductsWithoutInventoryQuery, IList<Product>>(
                new GetActiveProductsWithoutInventoryQuery(), cancellationToken);

            var model = new InventoryModel
            {
                ProductOptions = products.Select(p => new ProductOptionModel { Id = p.Id, Name = p.ProductName }).ToList()
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<InventoryAddCommand>(model);
                    await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Stock record successfully created.", Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Index));
                }
                catch (DuplicateDataException oex)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to create stock record.";
                    _logger.LogError(ex, errorMessage);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
                }
            }

            var products = await _mediator.SendQueryAsync<GetActiveProductsWithoutInventoryQuery, IList<Product>>(
                new GetActiveProductsWithoutInventoryQuery(), cancellationToken);
            model.ProductOptions = products.Select(p => new ProductOptionModel { Id = p.Id, Name = p.ProductName }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.SendQueryAsync(new GetInventoryByIdQuery { Id = id }, cancellationToken);

                if (result is null)
                    throw new Exception("Failed to load stock record");

                var model = _mapper.Map<InventoryModel>(result);
                return View(model);
            }
            catch (Exception ex)
            {
                const string errorMessage = "Failed to load stock record.";
                _logger.LogError(ex, errorMessage);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(InventoryModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<InventoryUpdateCommand>(model);
                    await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Stock record successfully updated.", Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to update stock record.";
                    _logger.LogError(ex, errorMessage);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetPagedStocks([FromBody] StockListModel model, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllInventoriesByPagingQuery
                {
                    PageIndex = model.Start / model.Length + 1,
                    PageSize = model.Length,
                    SearchText = model.Search.Value,
                    SortText = model.FormatSortExpression("Product.ProductName", "Quantity", "ReorderLevel", "UpdatedAt"),
                    LowStockOnly = model.LowStockOnly
                };

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync<GetAllInventoriesByPagingQuery,
                    (IList<Inventory>, int, int)>(query, cancellationToken);

                var result = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                                HttpUtility.HtmlEncode(item.Product?.ProductName ?? ""),
                                HttpUtility.HtmlEncode(item.Product?.SKU ?? ""),
                                item.Quantity.ToString(),
                                item.ReorderLevel.ToString(),
                                item.Quantity <= item.ReorderLevel ? "Low Stock" : "OK",
                                item.ProductId.ToString(),
                                item.Id.ToString()
                            }).ToArray()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get stock list");
                return Json(FashionHouse.Domain.Utilities.DataTables.EmptyResult);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(StockAdjustModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = new InventoryAdjustStockCommand
                    {
                        ProductId = model.ProductId,
                        ChangeQuantity = model.ChangeQuantity,
                        ReorderLevel = model.ReorderLevel
                    };

                    await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Stock successfully updated.", Type = ResponseTypes.Success });
                }
                catch (InsufficientStockException iex)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = iex.Message, Type = ResponseTypes.Danger });
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to update stock.";
                    _logger.LogError(ex, errorMessage);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}