using System.ComponentModel.DataAnnotations;

namespace FashionHouse.Web.Models.Account
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}
