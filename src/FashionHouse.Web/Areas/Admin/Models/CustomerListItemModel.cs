namespace FashionHouse.Web.Areas.Admin.Models
{
    public class CustomerListItemModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; } = true;
    }
}