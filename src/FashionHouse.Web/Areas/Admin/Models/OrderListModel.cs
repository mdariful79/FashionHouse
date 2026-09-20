using FashionHouse.Domain.Utilities;
using FashionHouse.Domain.Enums;

namespace FashionHouse.Web.Areas.Admin.Models
{
    public class OrderListModel : DataTables
    {
        public OrderStatus? StatusFilter { get; set; }
    }
}