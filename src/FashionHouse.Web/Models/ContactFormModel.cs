using System.ComponentModel.DataAnnotations;

namespace FashionHouse.Web.Models
{
    public class ContactFormModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Message { get; set; } = string.Empty;
        public string? Website { get; set; }
    }
}
