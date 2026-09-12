namespace FashionHouse.Web.Areas.Admin.Models
{
    public class SubCategoryListItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}