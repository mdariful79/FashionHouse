using FashionHouse.Domain.Utilities;

namespace FashionHouse.Web.Areas.Admin.Models
{
    public class StockListModel : DataTables
    {
        public bool LowStockOnly { get; set; }
    }
}
