using Cortex.Mediator;
using FashionHouse.Application.Contracts.Services;
using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Categories.Query;
using FashionHouse.Application.Features.ProductImages.Command;
using FashionHouse.Application.Features.ProductImages.Query;
using FashionHouse.Application.Features.Products.Command;
using FashionHouse.Application.Features.Products.Query;
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
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public ProductsController(ILogger<ProductsController> logger, IMediator mediator, IMapper mapper, IFileStorageService fileStorageService)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new ProductModel
            {
                CategoryOptions = await GetCategoryOptions(cancellationToken)
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<ProductAddCommand>(model);
                    var createdProduct = await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Product successfully created. You can now add images.", Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Update), new { id = createdProduct.Id });
                }
                catch (DuplicateDataException oex)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to create product.";
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

            model.CategoryOptions = await GetCategoryOptions(cancellationToken);
            model.SubCategoryOptions = await GetSubCategoryOptions(model.CategoryId, cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetProductByIdQuery { Id = id };
                var result = await _mediator.SendQueryAsync(query, cancellationToken);

                if (result != null)
                {
                    var model = _mapper.Map<ProductModel>(result);
                    model.CategoryOptions = await GetCategoryOptions(cancellationToken);
                    model.SubCategoryOptions = await GetSubCategoryOptions(result.CategoryId, cancellationToken);
                    return View(model);
                }
                else
                    throw new Exception("Failed to load product");
            }
            catch (Exception ex)
            {
                const string errorMessage = "Failed to load product.";
                _logger.LogError(ex, errorMessage);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<ProductUpdateCommand>(model);
                    await _mediator.SendCommandAsync(command, cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Product successfully updated.", Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Index));
                }
                catch (DuplicateDataException oex)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
                }
                catch (Exception ex)
                {
                    const string errorMessage = "Failed to update product.";
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

            model.CategoryOptions = await GetCategoryOptions(cancellationToken);
            model.SubCategoryOptions = await GetSubCategoryOptions(model.CategoryId, cancellationToken);
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetPagedProducts([FromBody] ProductListModel model, CancellationToken cancellationToken)
        {
            try
            {
                var query = _mapper.Map<GetAllProductsByPagingQuery>(model);
                query.SearchText = model.Search.Value;
                query.SortText = model.FormatSortExpression("ProductName", "ProductName", "SKU", "Price", "IsActive");

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync<GetAllProductsByPagingQuery,
                    (IList<Product>, int, int)>(query, cancellationToken);

                var productIds = items.Select(x => x.Id);
                var primaryImages = await _mediator.SendQueryAsync<GetPrimaryImagesByProductIdsQuery, IDictionary<Guid, ProductImage>>(
                    new GetPrimaryImagesByProductIdsQuery { ProductIds = productIds }, cancellationToken);

                var product = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = (from item in items
                            select new string[]
                            {
                        primaryImages.TryGetValue(item.Id, out var img) ? $"/uploads/products/{img.ImageUrl}" : "",
                        HttpUtility.HtmlEncode(item.ProductName),
                        HttpUtility.HtmlEncode(item.SKU),
                        $"৳{item.Price:N2}",
                        item.IsActive ? "Active" : "Inactive",
                        item.Id.ToString()
                            }).ToArray()
                };

                return Json(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get product list");
                return Json(FashionHouse.Domain.Utilities.DataTables.EmptyResult);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetSubCategoriesByCategory(Guid categoryId, CancellationToken cancellationToken)
        {
            var subCategories = await _mediator.SendQueryAsync<GetSubCategoriesByCategoryIdQuery, IList<SubCategory>>(
                new GetSubCategoriesByCategoryIdQuery { CategoryId = categoryId }, cancellationToken);

            var result = subCategories.Select(s => new { id = s.Id, name = s.Name });
            return Json(result);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var deleteCommand = new ProductDeleteCommand { Id = id };
                await _mediator.SendCommandAsync(deleteCommand, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Product successfully deleted.", Type = ResponseTypes.Success });
            }
            catch (Exception ex)
            {
                const string errorMessage = "Failed to delete product.";
                _logger.LogError(ex, errorMessage);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = errorMessage, Type = ResponseTypes.Danger });
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<CategoryOptionModel>> GetCategoryOptions(CancellationToken cancellationToken)
        {
            var categories = await _mediator.SendQueryAsync<GetActiveCategoriesQuery, IList<Category>>(
                new GetActiveCategoriesQuery(), cancellationToken);
            return categories.Select(c => new CategoryOptionModel { Id = c.Id, Name = c.Name }).ToList();
        }

        private async Task<List<SubCategoryOptionModel>> GetSubCategoryOptions(Guid categoryId, CancellationToken cancellationToken)
        {
            var subCategories = await _mediator.SendQueryAsync<GetSubCategoriesByCategoryIdQuery, IList<SubCategory>>(
                new GetSubCategoriesByCategoryIdQuery { CategoryId = categoryId }, cancellationToken);
            return subCategories.Select(s => new SubCategoryOptionModel { Id = s.Id, CategoryId = s.CategoryId, Name = s.Name }).ToList();
        }
    }
}