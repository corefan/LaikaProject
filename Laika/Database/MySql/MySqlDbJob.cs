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
