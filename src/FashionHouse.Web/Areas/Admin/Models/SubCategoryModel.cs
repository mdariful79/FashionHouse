using System.ComponentModel.DataAnnotations;

namespace FashionHouse.Web.Areas.Admin.Models
{
    public class SubCategoryModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<CategoryOptionModel> CategoryOptions { get; set; } = new();
    }

    public class CategoryOptionModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}