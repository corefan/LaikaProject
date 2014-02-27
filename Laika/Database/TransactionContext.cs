using System;
using System.Data.Common;

namespace Laika.Database
{
    /// <summary>
    /// Transaction 관리 클래스
    /// </summary>
    public abstract class TransactionContext
    {
        /// <summary>
        /// Transaction 관리 인스턴스 생성
        /// </summary>
        /// <param name="c"></param>
        public TransactionContext(DbConnection c)
        {
            connection = c;

            Transaction = c.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
        }

        /// <summary>
        /// MySqlCommand 생성
        /// </summary>
        /// <param name="query">수행될 쿼리</param>
        /// <returns>MySqlCommand 인스턴스</returns>
        //public DbCommand CreateDbCommand(string query)
        //{
        //    var command = connection.CreateCommand();
        //    command.Connection = connection;
        //    command.Transaction = Transaction;
        //    command.CommandText = query;
        //    return command;
        //}

        protected abstract DbCommand CreateDbCommand(string query);

        internal void Commit()
        {
            if (IsTransactionRollbackNeed == false)
                Transaction.Commit();
            else
                throw new Exception("Transaction rollback need.");
        }

        internal void Rollback()
        {
            Transaction.Rollback();
        }

        internal DbTransaction Transaction { get; private set; }
        protected DbConnection connection;
        public bool IsTransactionRollbackNeed = false;
    }
}
