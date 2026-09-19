using System.ComponentModel.DataAnnotations;

namespace FashionHouse.Web.Areas.Admin.Models
{
    public class StockAdjustModel
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public int ChangeQuantity { get; set; }
        public int? ReorderLevel { get; set; }
    }
}
