namespace FashionHouse.Web.Areas.Admin.Models
{
    public class HomeModel
    {

        public int CategoryCount { get; set; }
        public int SubCategoryCount { get; set; }
        public int ProductCount { get; set; }
        public int StockCount { get; set; }

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
