//-----------------------------------------------------------------------
// <copyright file="DataTableExtensions.cs" company="Lifeprojects.de">
//     Class: DataTableExtensions
//     Copyright © Lifeprojects.de 2016
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>1.1.2016</date>
//
// <summary>Extension Class</summary>
//-----------------------------------------------------------------------

namespace System.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    public static class DataTableExtensions
    {
        public static bool IsNullOrEmpty(this DataTable @this)
        {
            return @this == null && @this.AsEnumerable().Any() == false;
        }

        public static bool IsNotNullOrEmpty(this DataTable @this)
        {
            return @this != null && @this.AsEnumerable().Any() == true;
        }

        public static bool HasColumn(this DataTable @this, string columnName)
        {
            bool result = false;
            if (@this != null && !string.IsNullOrEmpty(columnName) && @this.Columns.IndexOf(columnName) >= 0)
            {
                result = true;
            }

            return result;
        }

        public static void RenameColumn(this DataTable @this, string oldName, string newName)
        {
            if (@this != null && !string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName) && oldName != newName)
            {
                int idx = @this.Columns.IndexOf(oldName);
                if (idx > 0)
                {
                    @this.Columns[idx].ColumnName = newName;
                    @this.AcceptChanges();
                }
                else
                {
                    throw new ArgumentException($"Column '{oldName}' not found!");
                }
            }
        }

        public static void RemoveColumn(this DataTable @this, string columnName)
        {
            if (@this != null && !string.IsNullOrEmpty(columnName) && @this.Columns.IndexOf(columnName) >= 0)
            {
                int idx = @this.Columns.IndexOf(columnName);
                @this.Columns.RemoveAt(idx);
                @this.AcceptChanges();
            }
        }

        public static Dictionary<string, Type> ColumnsToDictionary(this DataTable @this)
        {
            return @this.Columns
                .Cast<DataColumn>()
                .AsEnumerable<DataColumn>()
                .ToDictionary<DataColumn, string, Type>(col => col.ColumnName, col => col.DataType);
        }

        /// <summary>
        /// A DataTable extension method that return the first row.
        /// </summary>
        /// <param name="this">The table to act on.</param>
        /// <returns>The first row of the table.</returns>
        public static DataRow FirstRow(this DataTable @this)
        {
            return @this.Rows[0];
        }

        /// <summary>
        /// the DataTable extension method returns a row that matches the criterion
        /// </summary>
        /// <param name="this">The table to act on.</param>
        /// <returns>The first row of the table.</returns>
        public static DataRow FindRow(this DataTable @this, Func<DataRow, bool> filterCondition)
        {
            return @this.AsEnumerable().Where(filterCondition).FirstOrDefault();
        }

        /// <summary>
        /// the DataTable extension method returns a row that matches the criterion
        /// </summary>
        /// <param name="this">The table to act on.</param>
        /// <returns>The first row of the table.</returns>
        public static DataRow[] FindRows(this DataTable @this, Func<DataRow, bool> filterCondition)
        {
            return @this.AsEnumerable().Where(filterCondition).ToArray();
        }

        /// <summary>A DataTable extension method that last row.</summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>A DataRow.</returns>
        public static DataRow LastRow(this DataTable @this)
        {
            return @this.Rows[@this.Rows.Count - 1];
        }

        public static DataTable AsDataTable<T>(this IEnumerable<T> @this)
        {
            var table = new DataTable();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in @this)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static List<T> ToListOf<T>(this DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            var objectProperties = typeof(T).GetProperties(flags);
            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();

                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
                {
                    properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                }
                return instanceOfT;
            }).ToList();

            return targetList;
        }

        #region ToSorting
        public static DataTable ToSorting(this DataTable dt, ListSortDirection direction, string colName)
        {
            DataTable dataTableOut = null;

            string sortOrder = string.Empty;

            if (direction == ListSortDirection.Ascending)
            {
                sortOrder = "ASC";
            }
            else
            {
                sortOrder = "DESC";
            }

            try
            {
                dt.DefaultView.Sort = $"{colName} {sortOrder}";
                dataTableOut = dt.DefaultView.ToTable();
            }
            catch (Exception ex)
            {
                throw new Exception("Sort: \n" + ex.Message);
            }

            return dataTableOut;
        }

        public static DataTable ToSorting(this DataTable @this, ListSortDirection direction, params string[] colName)
        {
            DataTable dataTableOut = null;

            string sortOrder = string.Empty;

            if (direction == ListSortDirection.Ascending)
            {
                sortOrder = "ASC";
            }
            else
            {
                sortOrder = "DESC";
            }

            try
            {
                string sortColumns = $"{string.Join(",", colName)} {sortOrder}";

                @this.DefaultView.Sort = sortColumns;
                dataTableOut = @this.DefaultView.ToTable();
            }
            catch (Exception ex)
            {
                throw new Exception("Sort: \n" + ex.Message);
            }

            return dataTableOut;
        }
        #endregion ToSorting

        public static Dictionary<string, string> GetColumnsName(this DataTable @this)
        {
            Dictionary<string, string> columnNames = null;

            if (@this != null)
            {
                columnNames = new Dictionary<string, string>();

                foreach (DataColumn item in @this.Columns)
                {
                    if (item.ColumnName.ToLower() == "flags")
                    {
                        columnNames.Add(item.ColumnName, string.Format("{{{{{0}}}}}", item.ColumnName));
                    }
                    else
                    {
                        columnNames.Add(item.ColumnName, item.DataType.Name);
                    }
                }
            }

            return columnNames;
        }

        /// <summary>
        /// Renove doublicate entry in DataTable
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <example>
        /// DataTable resultTable = mainTable.DistinctDataTable();
        /// </example>
        public static DataTable DistinctDataTable(this DataTable table)
        {
            var resultTable = table.Clone();
            IEnumerable<DataRow> uniqueElements = table.AsEnumerable().Distinct(DataRowComparer.Default);
            foreach (var row in uniqueElements)
            {
                resultTable.ImportRow(row);
            }
            return resultTable;
        }

        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        #region GetColumnDataType

        public static Type GetColumnDataType(this DataTable tbl, string ColumnName)
        {
            try
            {
                return tbl.Columns[ColumnName].DataType;
            }
            catch (Exception ex)
            {
                throw new Exception("GetColumnDataType: \n" + ex.Message);
            }
        }

        public static Type GetColumnDataType(this DataTable tbl, int ColumnIndex)
        {
            try
            {
                return tbl.Columns[ColumnIndex].DataType;
            }
            catch (Exception ex)
            {
                throw new Exception("GetColumnDataType: \n" + ex.Message);
            }
        }

        #endregion GetColumnDataType

        #region GetColumnValue
        public static T GetColumnValue<T>(this DataTable tbl, int ColInd, int RowInd)
        {
            try
            {
                object column = tbl.Rows[RowInd][ColInd];
                return column == DBNull.Value ? default(T) : (T)Convert.ChangeType(column, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public static T GetColumnValue<T>(this DataTable tbl, string ColumnName, int RowInd)
        {
            try
            {
                object column = tbl.Rows[RowInd][ColumnName];
                return column == DBNull.Value ? default(T) : (T)Convert.ChangeType(column, typeof(T));

            }
            catch
            {
                return default(T);
            }
        }
        #endregion GetColumnValue
    }
}