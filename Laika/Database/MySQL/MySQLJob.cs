using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySqlDB
{
    /// <summary>
    /// Job 클래스
    /// </summary>
    public class MySqlJob : IDBJob
    {
        public delegate void JobDelegate(MySqlConnection connection);
        public delegate void ExceptionJobDelegate(Exception exception);

        public MySqlJob.JobDelegate QueryJob { get; set; }
        public MySqlJob.TransactionJobDelegate TransactionQueryJob { get; set; }
        public ExceptionJobDelegate ExceptionJob { get; set; }

        public delegate void TransactionJobDelegate(TransactionConext transactionContext);

        /// <summary>
        /// 일반 쿼리 job 생성
        /// </summary>
        /// <param name="job">job 람다 메소드</param>
        /// <param name="ex">예외 처리 람다 메소드</param>
        /// <returns></returns>
        public static MySqlJob CreateMySQLJob(MySqlJob.JobDelegate job, ExceptionJobDelegate ex = null)
        {
            return new MySqlJob(job, ex);
        }

        /// <summary>
        /// Transaction job 생성
        /// </summary>
        /// <param name="job">job 람다 메소드</param>
        /// <param name="ex">예외처리 람다 메소드</param>
        /// <returns></returns>
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
