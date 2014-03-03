using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;

namespace Laika.Database.SqlServer
{
    /// <summary>
    /// SqlServer 데이터베이스
    /// </summary>
    public class DatabaseForSqlServer : DatabaseBase, IDisposable
    {
        private bool disposed = false;

        internal DatabaseForSqlServer(SqlConnectionStringBuilder connection)
            : this(connection.ToString())
        {
            
        }

        internal DatabaseForSqlServer(string connectionString)
            : base(connectionString)
        {
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            conn.Dispose();
        }

        ~DatabaseForSqlServer()
        {
            Dispose(false);
        }
        /// <summary>
        /// Dispose 패턴
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override DbConnection GetConnection()
        {
            SqlConnection c = new SqlConnection(_connectionString);
            c.Open();
            return c;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed == true)
                return;

            if (disposing == true)
                Clear();

            disposed = true;
        }

        private void Clear()
        {
            SqlConnection.ClearAllPools();
        }
    }
}
