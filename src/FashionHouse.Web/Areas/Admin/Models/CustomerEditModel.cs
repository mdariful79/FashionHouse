using System.ComponentModel.DataAnnotations;

namespace FashionHouse.Web.Areas.Admin.Models
{
    public class CustomerEditModel
    {
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}