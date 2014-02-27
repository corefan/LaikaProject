using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySqlDB
{
    public class MySqlTransactionContext : TransactionContext
    {
        /// <summary>
        /// Transaction 관리 인스턴스 생성
        /// </summary>
        /// <param name="c"></param>
        public MySqlTransactionContext(DbConnection c)
            :base(c)
        {

        }

        public MySqlCommand GetTransactionCommand(string query)
        {
            return (MySqlCommand)CreateDbCommand(query);
        }

        protected override DbCommand CreateDbCommand(string query)
        {
            var command = connection.CreateCommand();
            command.Connection = connection;
            command.Transaction = Transaction;
            command.CommandText = query;
            return command;
        }
    }
}
