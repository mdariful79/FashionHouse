using FashionHouse.Domain.Entities;

namespace FashionHouse.Web.Models.AccountDashboard
{
    public class AccountDashboardModel
    {
        public string FullName { get; set; } = string.Empty;
        public IList<Order> RecentOrders { get; set; } = new List<Order>();
    }
}