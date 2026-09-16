namespace FashionHouse.Domain.Utilities
{
    public abstract class DataTables
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public SortColumn[] Order { get; set; }
        public DataTablesSearch Search { get; set; }

        public int PageIndex => Length > 0 ? (Start / Length) + 1 : 1;

        public int PageSize => Length == 0 ? 10 : Length;

        public static object EmptyResult => new
        {
            recordsTotal = 0,
            recordsFiltered = 0,
            data = Array.Empty<string>()
        };

        public string? FormatSortExpression(params string[] columns)
        {
            if (Order == null || Order.Length == 0)
                return null;

            var columnBuilder = new System.Text.StringBuilder();
            var appended = false;

            for (int i = 0; i < Order.Length; i++)
            {
                var columnIndex = Order[i].Column;

                if (columnIndex < 0 || columnIndex >= columns.Length || string.IsNullOrWhiteSpace(columns[columnIndex]))
                    continue;

                if (appended)
                    columnBuilder.Append(", ");

                columnBuilder.Append(columns[columnIndex])
                    .Append(" ")
                    .Append(Order[i].Dir);

                appended = true;
            }

            var orderString = columnBuilder.ToString();
            return orderString == string.Empty ? null : orderString;
        }
    }

    public struct SortColumn
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public struct DataTablesSearch
    {
        public bool Regex { get; set; }
        public string Value { get; set; }
    }
}