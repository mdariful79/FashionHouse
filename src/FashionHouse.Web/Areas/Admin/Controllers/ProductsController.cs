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
        private readonly IWebHostEnvironment _environment;

        public ProductsController(ILogger<ProductsController> logger, IMediator mediator, IMapper mapper, IFileStorageService fileStorageService, IWebHostEnvironment environment)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _environment = environment;
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
        public async Task<IActionResult> Create(ProductModel model, List<IFormFile> imageFiles, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = _mapper.Map<ProductAddCommand>(model);
                    var createdProduct = await _mediator.SendCommandAsync(command, cancellationToken);

                    var (savedCount, skippedCount) = await SaveProductImages(createdProduct.Id, imageFiles, cancellationToken);

                    var message = "Product successfully created.";
                    if (savedCount > 0)
                        message += $" {savedCount} image(s) uploaded.";
                    if (skippedCount > 0)
                        message += $" {skippedCount} file(s) skipped (invalid type or too large).";

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = message, Type = ResponseTypes.Success });

                    return RedirectToAction(nameof(Index)); // was: RedirectToAction(nameof(Update), new { id = createdProduct.Id });
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

        // ---------------- Product Images ----------------

        [HttpGet]
        public async Task<JsonResult> GetProductImages(Guid productId, CancellationToken cancellationToken)
        {
            var images = await _mediator.SendQueryAsync<GetProductImagesByProductIdQuery, IList<ProductImage>>(
                new GetProductImagesByProductIdQuery { ProductId = productId }, cancellationToken);

            var result = images
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new
                {
                    id = i.Id,
                    url = $"/uploads/products/{i.ImageUrl}",
                    isPrimary = i.IsPrimary,
                    displayOrder = i.DisplayOrder
                });

            return Json(result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProductImages(Guid productId, List<IFormFile> files, CancellationToken cancellationToken)
        {
            var (savedCount, skippedCount) = await SaveProductImages(productId, files, cancellationToken);

            if (savedCount == 0)
                return BadRequest("None of the selected files could be saved. Check file type (jpg/jpeg/png/webp) and size (max 5MB).");

            var images = await _mediator.SendQueryAsync<GetProductImagesByProductIdQuery, IList<ProductImage>>(
                new GetProductImagesByProductIdQuery { ProductId = productId }, cancellationToken);

            return Json(new
            {
                success = true,
                savedCount,
                skippedCount,
                images = images.OrderBy(i => i.DisplayOrder).Select(i => new
                {
                    id = i.Id,
                    url = $"/uploads/products/{i.ImageUrl}",
                    isPrimary = i.IsPrimary
                })
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductImage(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new DeleteProductImageCommand { Id = id }, cancellationToken);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product image {ImageId}", id);
                return Json(new { success = false, message = "Failed to delete image." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPrimaryProductImage(Guid id, Guid productId, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new SetPrimaryProductImageCommand { Id = id, ProductId = productId }, cancellationToken);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set primary image {ImageId} for product {ProductId}", id, productId);
                return Json(new { success = false, message = "Failed to set primary image." });
            }
        }

        private async Task<(int savedCount, int skippedCount)> SaveProductImages(
            Guid productId, List<IFormFile>? files, CancellationToken cancellationToken)
        {
            if (files == null || files.Count == 0)
                return (0, 0);

            var existingImages = await _mediator.SendQueryAsync<GetProductImagesByProductIdQuery, IList<ProductImage>>(
                new GetProductImagesByProductIdQuery { ProductId = productId }, cancellationToken);

            var hasPrimary = existingImages.Any(i => i.IsPrimary);
            var nextOrder = existingImages.Count == 0 ? 0 : existingImages.Max(i => i.DisplayOrder) + 1;

            var savedCount = 0;
            var skippedCount = 0;

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                string savedFileName;
                try
                {
                    await using var stream = file.OpenReadStream();
                    savedFileName = await _fileStorageService.SaveImageAsync(stream, file.FileName, "products", cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Skipped file {FileName}: {Message}", file.FileName, ex.Message);
                    skippedCount++;
                    continue;
                }

                var isPrimary = !hasPrimary && savedCount == 0;

                await _mediator.SendCommandAsync(new AddProductImageCommand
                {
                    ProductId = productId,
                    ImageUrl = savedFileName,
                    IsPrimary = isPrimary,
                    DisplayOrder = nextOrder++
                }, cancellationToken);

                if (isPrimary) hasPrimary = true;
                savedCount++;
            }

            return (savedCount, skippedCount);
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
