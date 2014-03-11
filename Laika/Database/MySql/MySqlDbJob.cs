using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySql
{
    /// <summary>
    /// MySql 작업단위 클래스
    /// </summary>
    public class MySqlDbJob : DbJobBase
    {
        /// <summary>
        /// multiple querys job
        /// </summary>
        /// <param name="querys">단일 쿼리 모음</param>
        /// <param name="ex">예외처리 Action</param>
        /// <returns>작업 인스턴스</returns>
        public static MySqlDbJob CreateExecuteNonQuerys(List<string> querys, Action<Exception> ex = null)
        {
            string query = string.Join(";", querys);
            return CreateExecuteNonQuery(query, ex);
        }
        /// <summary>
        /// sigle query job
        /// </summary>
        /// <param name="query">query string</param>
        /// <param name="ex">예외처리 Action</param>
        /// <returns>작업 인스턴스</returns>
        public static MySqlDbJob CreateExecuteNonQuery(string query, Action<Exception> ex = null)
        {
            Action<MySqlConnection> job = (connection) => 
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            };

            return new MySqlDbJob(job, ex);
        }
        /// <summary>
        /// 일반 쿼리 작업 용도의 인스턴스 생성
        /// </summary>
        /// <param name="job">작업 처리 Action</param>
        /// <param name="ex">예외 처리 Action</param>
        /// <returns>작업 인스턴스</returns>
        public static MySqlDbJob CreateMySqlJob(Action<MySqlConnection> job, Action<Exception> ex = null)
        {
            return new MySqlDbJob(job, ex);
        }
        /// <summary>
        /// 트랜잭션 쿼리 작업 용도의 인스턴스 생성
        /// </summary>
        /// <param name="job">작업 처리 Action</param>
        /// <param name="ex">예외 처리 Action</param>
        /// <returns>작업 인스턴스</returns>
        public static MySqlDbJob CreateMySqlTransactionJob(Action<MySqlTransactionContext> job, Action<Exception> ex = null)
        {
            return new MySqlDbJob(job, ex);
        }

        MySqlDbJob(Action<MySqlConnection> job, Action<Exception> ex)
        {
            _mySqlJob = job;
            _exception = ex;
        }

        MySqlDbJob(Action<MySqlTransactionContext> job, Action<Exception> ex)
        {
            _mySqlTransactionJob = job;
            _exception = ex;
        }

        protected override void TransactionContextDispose()
        {
            MySqlTransactionContext mysqlContext = _transaction as MySqlTransactionContext;
            if (mysqlContext != null)
                mysqlContext.Dispose();
        }
    }
}
