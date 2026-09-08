using Cortex.Mediator;
using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Categories.Command;
using FashionHouse.Application.Features.Categories.Query;
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
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CategoriesController(ILogger<CategoriesController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var model = new CategoryModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<CategoryAddCommand>(model);
                    await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Category successfully created.", Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Index));
                }
                catch (DuplicateDataException oex)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to create category.";
                    _logger.LogError(ex, errorMessage);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
                }
            }
            else
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Please provide all information.", Type = ResponseTypes.Warning });
            }

            return View(model);
        }

        public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetCategoryByIdQuery { Id = id };
                var result = await _mediator.SendQueryAsync(query, cancellationToken);

                if (result != null)
                {
                    var model = _mapper.Map<CategoryModel>(result);
                    return View(model);
                }
                else
                    throw new Exception("Failed to load category");
            }
            catch (Exception ex)
            {
                const string errorMessage = "Failed to load category.";
                _logger.LogError(ex, errorMessage);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<CategoryUpdateCommand>(model);
                    await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Category successfully updated.", Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Index));
                }
                catch (DuplicateDataException oex)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to update category.";
                    _logger.LogError(ex, errorMessage);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
                }
            }
            else
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Please provide all information.", Type = ResponseTypes.Warning });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetPagedCategories([FromBody] CategoryListModel model, CancellationToken cancellationToken)
        {
            try
            {
                var query = _mapper.Map<GetAllCategoriesByPagingQuery>(model);
                query.SearchText = model.Search.Value;
                query.SortText = model.FormatSortExpression("Name", "Name", "Slug", "IsActive");

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync<GetAllCategoriesByPagingQuery,
                    (IList<Category>, int, int)>(query, cancellationToken);

                var category = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                                HttpUtility.HtmlEncode(item.Name),
                                HttpUtility.HtmlEncode(item.Slug),
                                item.IsActive ? "Active" : "Inactive",
                                item.Id.ToString()
                            }).ToArray()
                };

                return Json(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get category list");
                return Json(FashionHouse.Domain.Utilities.DataTables.EmptyResult);
            }
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var deleteCommand = new CategoryDeleteCommand { Id = id };
                await _mediator.SendCommandAsync(deleteCommand, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Category successfully deleted.", Type = ResponseTypes.Success });
            }
            catch (Exception ex)
            {
                const string errorMessage = "Failed to delete category.";
                _logger.LogError(ex, errorMessage);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}