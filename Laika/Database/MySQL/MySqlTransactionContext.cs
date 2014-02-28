using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySql
{
    /// <summary>
    /// Transaction 처리 Context
    /// </summary>
    public class MySqlTransactionContext : TransactionContextBase, IDisposable
    {
        private bool disposed = false;
        internal MySqlTransactionContext(MySqlConnection connection)
            : base(connection)
        { 
        
        }

        ~MySqlTransactionContext()
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
        /// DB Command 생성
        /// </summary>
        /// <param name="query">쿼리문</param>
        /// <returns>DB Command</returns>
        public MySqlCommand CreateDbCommand(string query)
        {
            return (MySqlCommand)CreateCommand(query);
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
            ((MySqlTransaction)Transaction).Dispose();
            ((MySqlConnection)Connection).Dispose();
        }
    }
}
