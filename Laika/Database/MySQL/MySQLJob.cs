using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySqlDB
{
    public class MySqlJob : IDBJob
    {
        public delegate void JobDelegate(MySqlConnection connection);
        public delegate void ExceptionJobDelegate(Exception exception);

        public MySqlJob.JobDelegate QueryJob { get; set; }
        public MySqlJob.TransactionJobDelegate TransactionQueryJob { get; set; }
        public ExceptionJobDelegate ExceptionJob { get; set; }

        public delegate void TransactionJobDelegate(TransactionConext transactionContext);


        public static MySqlJob CreateMySQLJob(MySqlJob.JobDelegate job, ExceptionJobDelegate ex = null)
        {
            return new MySqlJob(job, ex);
        }

        public static MySqlJob CreateMySQLTransactionJob(MySqlJob.TransactionJobDelegate job, ExceptionJobDelegate ex = null)
        {
            return new MySqlJob(job, ex);
        }

        MySqlJob(MySqlJob.JobDelegate job, MySqlJob.ExceptionJobDelegate ex)
        {
            QueryJob = job;
            ExceptionJob = ex;
        }

        MySqlJob(MySqlJob.TransactionJobDelegate job, MySqlJob.ExceptionJobDelegate ex)
        {
            TransactionQueryJob = job;
            ExceptionJob = ex;
        }
    }
}
