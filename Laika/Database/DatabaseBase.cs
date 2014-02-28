using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
namespace Laika.Database
{
    /// <summary>
    /// 데이터베이스 베이스 클래스
    /// </summary>
    public abstract class DatabaseBase : IDatabase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="connectionString">Connection 스트링</param>
        public DatabaseBase(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// 동기 작업 수행
        /// </summary>
        /// <param name="job">작업 인스턴스</param>
        public void DoJob(DbJobBase job)
        {
            job.DoJob(GetConnection());
        }
        /// <summary>
        /// 비동기 작업 수행
        /// </summary>
        /// <param name="job">작업 인스턴스</param>
        /// <returns>비동기 Task</returns>
        public Task AsyncDoJob(DbJobBase job)
        {
            return Task.Factory.StartNew(() => 
            {
                job.DoJob(GetConnection());
            });
        }

        protected abstract DbConnection GetConnection();
        protected string _connectionString;
    }
}
