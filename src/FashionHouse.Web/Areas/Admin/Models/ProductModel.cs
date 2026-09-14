using System.ComponentModel.DataAnnotations;

namespace FashionHouse.Web.Areas.Admin.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Sub category is required")]
        public Guid SubCategoryId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discounted price cannot be negative")]
        public decimal? DiscountedPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public List<CategoryOptionModel> CategoryOptions { get; set; } = new();
        public List<SubCategoryOptionModel> SubCategoryOptions { get; set; } = new();
    }

    public class SubCategoryOptionModel
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}