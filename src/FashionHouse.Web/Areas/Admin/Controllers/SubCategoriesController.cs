using Cortex.Mediator;
using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Categories.Query;
using FashionHouse.Application.Features.SubCategories.Command;
using FashionHouse.Application.Features.SubCategories.Query;
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
    public class SubCategoriesController : Controller
    {
        private readonly ILogger<SubCategoriesController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SubCategoriesController(ILogger<SubCategoriesController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var categories = await _mediator.SendQueryAsync<GetActiveCategoriesQuery, IList<Category>>(
                new GetActiveCategoriesQuery(), cancellationToken);

            var model = new SubCategoryModel
            {
                CategoryOptions = categories.Select(c => new CategoryOptionModel { Id = c.Id, Name = c.Name }).ToList()
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<SubCategoryAddCommand>(model);
                    await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Sub category successfully created.", Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Index));
                }
                catch (DuplicateDataException oex)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to create sub category.";
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

            var categories = await _mediator.SendQueryAsync<GetActiveCategoriesQuery, IList<Category>>(
                new GetActiveCategoriesQuery(), cancellationToken);
            model.CategoryOptions = categories.Select(c => new CategoryOptionModel { Id = c.Id, Name = c.Name }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSubCategoryByIdQuery { Id = id };
                var result = await _mediator.SendQueryAsync(query, cancellationToken);

                if (result != null)
                {
                    var model = _mapper.Map<SubCategoryModel>(result);

                    var categories = await _mediator.SendQueryAsync<GetActiveCategoriesQuery, IList<Category>>(
                        new GetActiveCategoriesQuery(), cancellationToken);
                    model.CategoryOptions = categories.Select(c => new CategoryOptionModel { Id = c.Id, Name = c.Name }).ToList();

                    return View(model);
                }
                else
                    throw new Exception("Failed to load sub category");
            }
            catch (Exception ex)
            {
                const string errorMessage = "Failed to load sub category.";
                _logger.LogError(ex, errorMessage);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SubCategoryModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<SubCategoryUpdateCommand>(model);
                    await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Sub category successfully updated.", Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Index));
                }
                catch (DuplicateDataException oex)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to update sub category.";
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

            var categories = await _mediator.SendQueryAsync<GetActiveCategoriesQuery, IList<Category>>(
                new GetActiveCategoriesQuery(), cancellationToken);
            model.CategoryOptions = categories.Select(c => new CategoryOptionModel { Id = c.Id, Name = c.Name }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetPagedSubCategories([FromBody] SubCategoryListModel model, CancellationToken cancellationToken)
        {
            try
            {
                var query = _mapper.Map<GetAllSubCategoriesByPagingQuery>(model);
                query.SearchText = model.Search.Value;
                query.SortText = model.FormatSortExpression("Name", "Name", "Slug", "IsActive");

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync<GetAllSubCategoriesByPagingQuery,
                    (IList<SubCategory>, int, int)>(query, cancellationToken);

                var subCategory = new
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

                return Json(subCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sub category list");
                return Json(FashionHouse.Domain.Utilities.DataTables.EmptyResult);
            }
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var deleteCommand = new SubCategoryDeleteCommand { Id = id };
                await _mediator.SendCommandAsync(deleteCommand, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Sub category successfully deleted.", Type = ResponseTypes.Success });
            }
            catch (Exception ex)
            {
                const string errorMessage = "Failed to delete sub category.";
                _logger.LogError(ex, errorMessage);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}