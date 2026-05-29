//-----------------------------------------------------------------------
// <copyright file="DatabaseService.cs" company="Lifeprojects.de">
//     Class: DatabaseService
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>22.04.2025</date>
//
// <summary>Database Service for SQLite Database</summary>
//-----------------------------------------------------------------------

namespace System.Data.SQLite
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    public class DatabaseService : IDisposable
    {
        private bool classIsDisposed;

        public DatabaseService(string fullPath)
        {
            this.FullName = fullPath;
            this.Database = Path.GetFileName(fullPath);
            this.SqlConnectionString = this.ConnectStringToText(this.FullName);
        }

        ~DatabaseService()
        {
            if (this.Connection != null)
            {
                this.FullName = null;
                this.Database = null;
                this.SqlConnectionString = null;
                this.IsOpen = false;
                this.Connection = null;
            }
        }

        public string FullName { get; private set; }

        public string Database { get; private set; }

        public string SqlConnectionString { get; private set; }

        public bool IsOpen { get; private set; }

        public SQLiteConnection Connection { get; private set; }


        public void Create()
        {
            try
            {
                if (File.Exists(this.FullName) == false)
                {
                    SQLiteConnection.CreateFile(this.FullName);

                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
                    {
                        if (sqliteConnection.State != ConnectionState.Open)
                        {
                            sqliteConnection.Open();
                            this.IsOpen = true;
                        }

                        sqliteConnection.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Create(Action<SQLiteConnection> actionMethod)
        {
            try
            {
                if (File.Exists(this.FullName) == false)
                {
                    SQLiteConnection.CreateFile(this.FullName);

                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
                    {
                        if (sqliteConnection.State != ConnectionState.Open)
                        {
                            sqliteConnection.Open();
                            this.IsOpen = true;
                        }

                        if (actionMethod != null)
                        {
                            actionMethod?.Invoke(sqliteConnection);
                        }

                        sqliteConnection.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SQLiteConnection OpenConnection()
        {
            try
            {
                if (File.Exists(this.FullName) == true)
                {
                    SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString);
                        if (sqliteConnection.State != ConnectionState.Open)
                    {
                        sqliteConnection.Open();
                        this.Connection = sqliteConnection;
                        this.IsOpen = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return this.Connection;
        }

        public void CloseConnection()
        {
            try
            {
                if (File.Exists(this.FullName) == true)
                {
                    if (this.Connection.State == ConnectionState.Open)
                    {
                        this.Connection.Close();
                        this.FullName = null;
                        this.Database = null;
                        this.SqlConnectionString = null;
                        this.IsOpen = false;
                        this.Connection = null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Insert(Action<SQLiteConnection> actionMethod)
        {
            try
            {
                if (File.Exists(this.FullName) == true)
                {
                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
                    {
                        if (sqliteConnection.State != ConnectionState.Open)
                        {
                            sqliteConnection.Open();
                            this.IsOpen = true;
                        }

                        if (actionMethod != null)
                        {
                            actionMethod?.Invoke(sqliteConnection);
                        }

                        sqliteConnection.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Insert(Action<SQLiteConnection, object> actionMethod, object parameter)
        {
            try
            {
                if (File.Exists(this.FullName) == true)
                {
                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
                    {
                        if (sqliteConnection.State != ConnectionState.Open)
                        {
                            sqliteConnection.Open();
                            this.IsOpen = true;
                        }

                        if (actionMethod != null)
                        {
                            actionMethod?.Invoke(sqliteConnection, parameter);
                        }

                        sqliteConnection.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteDatabaseFile()
        {
            try
            {
                if (File.Exists(this.FullName) == true)
                {
                    FileInfo fi = new FileInfo(this.FullName);
                    fi.Delete();

                    if (File.Exists(this.FullName) == false)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        public void Backup(string targetBackup = "")
        {
            try
            {
                if (File.Exists(this.FullName) == true)
                {
                    FileInfo fi = new FileInfo(this.FullName);
                    if (string.IsNullOrEmpty(targetBackup) == true)
                    {
                        targetBackup = $"{Path.GetDirectoryName(this.FullName)}\\{Path.GetFileNameWithoutExtension(this.FullName)}_{DateTime.Now.ToString("yyyyMMdd",CultureInfo.CurrentCulture)}{Path.GetExtension(this.FullName)}";
                    }

                    var result = this.CopyFileAsync(this.FullName, targetBackup);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lesen alle Columns/Tabelle der aktuellen SQLite Datenbank
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GetSchema(TableSchemaTyp schemaTyp = TableSchemaTyp.Columns)
        {
            DataTable dataTableSchema = null;

            try
            {
                using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
                {
                    if (sqliteConnection.State != ConnectionState.Open)
                    {
                        sqliteConnection.Open();
                        this.IsOpen = true;

                        dataTableSchema = new DataTable(schemaTyp.ToString());
                        dataTableSchema = sqliteConnection.GetSchema(schemaTyp.ToString());
                    }

                    sqliteConnection.Close();
                }

            }
            catch (Exception)
            {
                throw;
            }

            return dataTableSchema;
        }

        public DataTable Tables()
        {
            DataTable tables = null;

            try
            {
                using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
                {
                    tables = new DataTable("Schema");

                    if (sqliteConnection.State != ConnectionState.Open)
                    {
                        sqliteConnection.Open();
                        this.IsOpen = true;

                        tables = sqliteConnection.GetSchema(TableSchemaTyp.Columns.ToString());
                        tables.Columns.Remove("TABLE_CATALOG");
                        tables.Columns.Remove("TABLE_SCHEMA");
                        tables.Columns.Remove("COLUMN_GUID");
                        tables.Columns.Remove("COLUMN_PROPID");
                        tables.Columns.Remove("COLUMN_HASDEFAULT");
                        tables.Columns.Remove("COLUMN_DEFAULT");
                        tables.Columns.Remove("COLUMN_FLAGS");
                        tables.Columns.Remove("TYPE_GUID");
                        tables.Columns.Remove("CHARACTER_MAXIMUM_LENGTH");
                        tables.Columns.Remove("CHARACTER_SET_CATALOG");
                        tables.Columns.Remove("CHARACTER_SET_SCHEMA");
                        tables.Columns.Remove("CHARACTER_SET_NAME");
                        tables.Columns.Remove("COLLATION_CATALOG");
                        tables.Columns.Remove("COLLATION_NAME");
                        tables.Columns.Remove("DOMAIN_CATALOG");
                        tables.Columns.Remove("DOMAIN_NAME");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return tables;
        }

        public bool CheckIfColumnExists(string tableName, string columnName)
        {
            try
            {
                using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
                {
                    sqliteConnection.Open();
                    this.IsOpen = true;
                    var cmd = sqliteConnection.CreateCommand();
                    cmd.CommandText = string.Format(CultureInfo.CurrentCulture, $"PRAGMA table_info({tableName})");

                    var reader = cmd.ExecuteReader();
                    int nameIndex = reader.GetOrdinal("Name");
                    while (reader.Read())
                    {
                        if (reader.GetString(nameIndex).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    sqliteConnection.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return false;
        }

        public bool Exist()
        {
            if (File.Exists(this.FullName) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public long Length()
        {
            long result = 0;

            try
            {
                if (File.Exists(this.FullName) == true)
                {
                    FileInfo fi = new FileInfo(this.FullName);
                    result = fi.Length;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public DateTime LastWriteTime()
        {
            DateTime result = new DateTime(1900,1,1);

            if (File.Exists(this.FullName) == true)
            {
                FileInfo fi = new FileInfo(this.FullName);
                result = fi.LastWriteTime;
            }

            return result;
        }

        public DateTime CreationTime()
        {
            DateTime result = new DateTime(1900, 1, 1);

            if (File.Exists(this.FullName) == true)
            {
                FileInfo fi = new FileInfo(this.FullName);
                result = fi.CreationTime;
            }

            return result;
        }

        public void Vacuum()
        {
            using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
            {
                if (sqliteConnection.State != ConnectionState.Open)
                {
                    sqliteConnection.Open();
                    this.IsOpen = true;

                    using (SQLiteCommand cmd = new SQLiteCommand("vacuum", sqliteConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                sqliteConnection.Close();
            }
        }

        public string Version()
        {
            string result = string.Empty;

            using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
            {
                if (sqliteConnection.State != ConnectionState.Open)
                {
                    sqliteConnection.Open();
                    this.IsOpen = true;

                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT SQLITE_VERSION()", sqliteConnection))
                    {
                        result = cmd.ExecuteScalar().ToString();
                    }
                }

                sqliteConnection.Close();
            }

            return result;
        }

        public List<Tuple<string,string,object,Type>> MetadataInformation()
        {
            List<Tuple<string, string, object, Type>> meta = new List<Tuple<string, string, object, Type>>();

            FileInfo fi = new FileInfo(this.FullName);
            if (fi.Exists == true)
            {
                meta.Add(new Tuple<string, string,object, Type>("Name", "FileInfo", fi.Name, typeof(string)));
                meta.Add(new Tuple<string, string, object, Type>("Path", "FileInfo", fi.FullName, typeof(string)));
                meta.Add(new Tuple<string, string, object, Type>("Length", "FileInfo", fi.Length, typeof(long)));
                meta.Add(new Tuple<string, string, object, Type>("LastWriteTime", "FileInfo", fi.LastWriteTime, typeof(DateTime)));
            }

            using (SQLiteConnection sqliteConnection = new SQLiteConnection(this.SqlConnectionString))
            {
                if (sqliteConnection.State != ConnectionState.Open)
                {
                    sqliteConnection.Open();
                    this.IsOpen = true;

                    using (SQLiteCommand cmd = new SQLiteCommand("vacuum", sqliteConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                meta.Add(new Tuple<string, string, object, Type>("DataSource", "SQLiteConnection", sqliteConnection.DataSource, typeof(string)));
                meta.Add(new Tuple<string, string, object, Type>("DefaultTimeout", "SQLiteConnection", sqliteConnection.DefaultTimeout, typeof(int)));
                meta.Add(new Tuple<string, string, object, Type>("ServerVersion", "SQLiteConnection", sqliteConnection.ServerVersion, typeof(string)));

                sqliteConnection.Close();
            }

            return meta;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private string ConnectStringToText(string databasePath)
        {
            SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder();
            conString.DataSource = databasePath;
            conString.DefaultTimeout = 30;
            conString.SyncMode = SynchronizationModes.Off;
            conString.JournalMode = SQLiteJournalModeEnum.Memory;
            conString.PageSize = 65536;
            conString.CacheSize = 16777216;
            conString.FailIfMissing = false;
            conString.ReadOnly = false;
            conString.Version = 3;
            conString.UseUTF16Encoding = true;

            return conString.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }

        #region Implement Dispose

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool classDisposing = false)
        {
            if (this.classIsDisposed == false)
            {
                if (classDisposing == true)
                {
                    this.FullName = null;
                    this.Database = null;
                    this.SqlConnectionString = null;
                    this.IsOpen = false;
                    this.Connection = null;
                }
            }

            this.classIsDisposed = true;
        }

        #endregion Implement Dispose
    }
}
