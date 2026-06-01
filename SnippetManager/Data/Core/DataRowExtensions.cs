namespace System.Data
{
    using System.Text;

    public static class DataRowExtensions
    {
        public static string ToString(this DataRow @this, char separator = ',')
        {
            return string.Join(separator, @this.ItemArray.Select(c => c.ToString().Trim()).ToArray());
        }

        public static string ToString(this DataRow @this, string columns, char separator = ',')
        {
            StringBuilder sb = new StringBuilder();

            string[] columnList = columns.Split(',');

            foreach (string column in columnList)
            {
                if (@this.HasColumn(column) == true)
                {
                    sb.Append(@this[column].ToString());
                    sb.Append(separator);
                }
            }

            sb.Remove(sb.ToString().Trim().Length - 1, 1);

            return sb.ToString();
        }

        public static bool HasColumn(this DataRow @this, string columnName)
        {
            bool result = false;

            int columnFound = @this.Table.Columns.OfType<DataColumn>().ToList().Count(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (columnFound > 0)
            {
                result = true;
            }

            return result;
        }
    }
}
