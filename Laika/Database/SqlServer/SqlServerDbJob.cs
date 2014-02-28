using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Laika.Database.SqlServer;

namespace Laika.Database.SqlServer
{
    /// <summary>
    /// SqlServer 작업 클래스
    /// </summary>
    public class SqlServerDbJob : DbJobBase
    {
        /// <summary>
        /// SqlServer 생성 팩토리
        /// </summary>
        /// <param name="job">작업처리 Action</param>
        /// <param name="ex">예외처리 Action</param>
        /// <returns>작업 인스턴스</returns>
        public static SqlServerDbJob CreateSqlServerJob(Action<SqlConnection> job, Action<Exception> ex = null)
        {
            return new SqlServerDbJob(job, ex);
        }
        /// <summary>
        /// SqlServer 작업 생성 팩토리
        /// </summary>
        /// <param name="job">작업처리 Action</param>
        /// <param name="ex">예외처리 Action</param>
        /// <returns>작업 인스턴스</returns>
        public static SqlServerDbJob CreateSqlServerTransactionJob(Action<SqlServerTransactionContext> job, Action<Exception> ex)
        {
            return new SqlServerDbJob(job, ex);
        }

        SqlServerDbJob(Action<SqlConnection> job, Action<Exception> ex)
        {
            _sqlServerJob = job;
            _exception = ex;
        }
        SqlServerDbJob(Action<SqlServerTransactionContext> job, Action<Exception> ex)
        {
            _sqlServerTransactionJob = job;
            _exception = ex;
        }
    }
}
