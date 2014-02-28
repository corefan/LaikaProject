using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Laika.Database.SqlServer
{
    /// <summary>
    /// SqlServer 트랜잭션 Context
    /// </summary>
    public class SqlServerTransactionContext : TransactionContextBase, IDisposable
    {
        private bool disposed = false;
        internal SqlServerTransactionContext(SqlConnection connection)
            : base(connection)
        { }

        ~SqlServerTransactionContext()
        {
            Dispose(false);
        }
        /// <summary>
        /// Dispose 패턴
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 트랜잭션 DbCommand 생성
        /// </summary>
        /// <param name="query">쿼리</param>
        /// <returns>DbCommand</returns>
        public SqlCommand CreateDbCommand(string query)
        {
            return (SqlCommand)CreateCommand(query);
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
            ((SqlTransaction)Transaction).Dispose();
            ((SqlConnection)Connection).Dispose();
        }
    }
}
