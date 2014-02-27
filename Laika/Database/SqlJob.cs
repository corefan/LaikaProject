using System;
using System.Data.Common;
using Laika.Database.MySqlDB;
using Laika.Database.SqlServer;
using MySql.Data.MySqlClient;

namespace Laika.Database
{
    public class SqlJob : IDBJob
    {
        public delegate void MySqlJobDelegate(MySqlConnection connection);
        public delegate void MySqlTransactionJobDelegate(MySqlTransactionContext transactionContext);
        public MySqlJobDelegate MySqlQueryJob { get; set; }
        public MySqlTransactionJobDelegate MySqlTransactionQueryJob { get; set; }

        public delegate void SqlServerJobDelegate(DbConnection connection);
        public delegate void SqlServerTransactionJobDelegate(SqlServerTransactionContext transactionContext);
        public SqlServerJobDelegate QueryJob { get; set; }
        public SqlServerTransactionJobDelegate TransactionQueryJob { get; set; }

        public delegate void ExceptionJobDelegate(Exception exception);
        public ExceptionJobDelegate ExceptionJob { get; set; }  
    }
}
