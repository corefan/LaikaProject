using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Laika.Database.SqlServer
{
    /// <summary>
    /// SqlServer 작업 클래스
    /// </summary>
    public class SqlServerDbJob : DbJobBase
    {
        /// <summary>
        /// multiple querys job
        /// </summary>
        /// <param name="querys">단일 쿼리 모음</param>
        /// <param name="ex">예외처리 Action</param>
        /// <returns>작업 인스턴스</returns>
        public static SqlServerDbJob CreateExecuteNonQuerys(List<string> querys, Action<Exception> ex = null)
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
        public static SqlServerDbJob CreateExecuteNonQuery(string query, Action<Exception> ex = null)
        {
            Action<SqlConnection> job = (connection) => 
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            };

            return new SqlServerDbJob(job, ex);
        }
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

        protected override void TransactionContextDispose()
        {
            SqlServerTransactionContext sqlServerContext = _transaction as SqlServerTransactionContext;
            if (sqlServerContext != null)
                sqlServerContext.Dispose();
        }
    }
}
