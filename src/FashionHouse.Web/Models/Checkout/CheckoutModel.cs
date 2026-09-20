using FashionHouse.Domain.Entities;

namespace FashionHouse.Web.Models.Checkout
{
    public class CheckoutModel
    {
        public Cart Cart { get; set; } = null!;
        public IList<Address> Addresses { get; set; } = new List<Address>();
        public Guid SelectedAddressId { get; set; }
    }

    public class NewAddressModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public bool IsDefault { get; set; }
    }
}