using System.ComponentModel.DataAnnotations;

namespace FashionHouse.Web.Areas.Admin.Models
{
    public class InventoryModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product is required")]
        public Guid ProductId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder level cannot be negative")]
        public int ReorderLevel { get; set; } = 5;

        public string ProductName { get; set; } = string.Empty;

        public List<ProductOptionModel> ProductOptions { get; set; } = new();
    }

    public class ProductOptionModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}