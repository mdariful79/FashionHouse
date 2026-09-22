namespace FashionHouse.Web.Areas.Admin.Models
{
    public class HomeModel
    {
        // ...your existing properties (CategoryCount, ProductCount, etc.)...

        public IReadOnlyList<RecentOrderRow> RecentOrders { get; set; } = new List<RecentOrderRow>();
    }

    public class RecentOrderRow
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderedAt { get; set; }
        public int ItemCount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
