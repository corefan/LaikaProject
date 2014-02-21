using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySqlDB
{
    /// <summary>
    /// Transaction 관리 클래스
    /// </summary>
    public class TransactionConext
    {
        /// <summary>
        /// Transaction 관리 인스턴스 생성
        /// </summary>
        /// <param name="c"></param>
        public TransactionConext(MySqlConnection c)
        {
            connection = c;

            Transaction = c.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
        }

        /// <summary>
        /// MySqlCommand 생성
        /// </summary>
        /// <param name="query">수행될 쿼리</param>
        /// <returns>MySqlCommand 인스턴스</returns>
        public MySqlCommand CreateMySqlCommand(string query)
        {
            var command = connection.CreateCommand();
            command.Connection = connection;
            command.Transaction = Transaction;
            command.CommandText = query;
            return command;
        }

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

        internal MySqlTransaction Transaction { get; private set; }
        private MySqlConnection connection;
        public bool IsTransactionRollbackNeed = false;
    }
}
