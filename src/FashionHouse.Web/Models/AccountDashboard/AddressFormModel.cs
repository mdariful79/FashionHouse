using System.ComponentModel.DataAnnotations;

namespace FashionHouse.Web.Models.AccountDashboard
{
    public class AddressFormModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full address is required")]
        [Display(Name = "Full Address")]
        public string FullAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "District is required")]
        public string District { get; set; } = string.Empty;

        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Display(Name = "Set as default address")]
        public bool IsDefault { get; set; }
    }
}