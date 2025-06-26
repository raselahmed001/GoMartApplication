using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace GoMartApplication
{
    public class DBConnect : IDisposable
    {
        private readonly SqlConnection _connection;
        private bool _disposed = false;

       
        private const string ConnectionString =
            @"Data Source=DESKTOP-BDMKA4V\SQLEXPRESS;
              Initial Catalog=GoMartDB;
              Integrated Security=True;
              TrustServerCertificate=True";

        public DBConnect()
        {
            _connection = new SqlConnection(ConnectionString);
        }

        public SqlConnection GetConnection()
        {
            if (_disposed)
                throw new ObjectDisposedException("DBConnect");

            return _connection;
        }

        public void OpenConnection()
        {
            if (_disposed)
                throw new ObjectDisposedException("DBConnect");

            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (!_disposed && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        // IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Managed resources
                    if (_connection != null)
                    {
                        CloseConnection();
                        _connection.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        ~DBConnect()
        {
            Dispose(false);
        }
    }
}
