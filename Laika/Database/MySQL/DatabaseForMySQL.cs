using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySqlDB
{
    /// <summary>
    /// MySql 연결 및 작업 수행 객체입니다.
    /// </summary>
    public class DatabaseForMySql : IDatabase
    {
        /// <summary>
        /// DatabasaeForMySql 생성자
        /// </summary>
        /// <param name="host">서버</param>
        /// <param name="port">포트</param>
        /// <param name="user">사용자 ID</param>
        /// <param name="password">패스워드</param>
        /// <param name="database">DB명</param>
        /// <param name="minPool">최소 연결 개수</param>
        /// <param name="maxPool">최대 연결 개수</param>
        public DatabaseForMySql(string host, uint port, string user, string password, string database, uint minPool, uint maxPool)
        {
            if (minPool < 1 || maxPool < 1 || minPool > maxPool)
                throw new ArgumentException("Invalid DB pool size.");

            ConnectionStringBuilder = new MySqlConnectionStringBuilder();
            ConnectionStringBuilder.Server = host;
            ConnectionStringBuilder.Port = port;
            ConnectionStringBuilder.UserID = user;
            ConnectionStringBuilder.Password = password;
            ConnectionStringBuilder.Database = database;
            ConnectionStringBuilder.Pooling = true;
            ConnectionStringBuilder.MinimumPoolSize = minPool;
            ConnectionStringBuilder.MaximumPoolSize = maxPool;

            InitializeConnection();

            SetPool((int)((maxPool + minPool) / 2));
        }

        private void SetPool(int size)
        {
            if (Laika.ThreadPoolManager.AppDomainThreadPoolManager.IsSetMinThreadPool == false)
            {
                Laika.ThreadPoolManager.AppDomainThreadPoolManager.SetMinThreadPool(size, size);
            }
        }

        ~DatabaseForMySql()
        {
            Dispose(false);
        }

        private void InitializeConnection()
        {
            MySqlConnection conn = new MySqlConnection(ConnectionStringBuilder.ToString());
            conn.Open();
            conn.Dispose();
        }

        /// <summary>
        /// DB job 수행 비동기 메소드
        /// </summary>
        /// <param name="job">job 객체</param>
        /// <returns>job은 비동기로 작업을 하며, Task를 return 합니다.</returns>
        public Task DoJobAsync(IDBJob job)
        {
            if (job.QueryJob != null)
            {
                return NormalQueryJobAsync(job);
            }

            if (job.TransactionQueryJob != null)
            {
                return TransactionQueryJobAsync(job);
            }
            
            return null;
        }

        /// <summary>
        /// DB job 수행 메소드
        /// </summary>
        /// <param name="job">job 객체</param>
        /// <returns>job은 동기로 작업을 합니다.</returns>
        public void DoJob(IDBJob job)
        {
            if (job.QueryJob != null)
            {
                NormalQueryJobContext(job);
            }

            if (job.TransactionQueryJob != null)
            {
                TransactionQueryJobContext(job);
            }
        }

        private Task TransactionQueryJobAsync(IDBJob job)
        {
            Task result = Task.Factory.StartNew(() =>
            {
                TransactionQueryJobContext(job);
            });

            return result;
        }

        private void TransactionQueryJobContext(IDBJob job)
        {
            TransactionConext transaction = null;
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(ConnectionStringBuilder.ToString());
                connection.Open();
                transaction = new TransactionConext(connection);
                job.TransactionQueryJob(transaction);
                transaction.Commit();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                if (job.ExceptionJob != null)
                    job.ExceptionJob(ex);

                if (transaction != null)
                    transaction.Rollback();

                if (connection != null)
                    connection.Dispose();
            }
        }

        private Task NormalQueryJobAsync(IDBJob job)
        {
            Task result = Task.Factory.StartNew(() =>
            {
                NormalQueryJobContext(job);
            });
            return result;
        }

        private void NormalQueryJobContext(IDBJob job)
        {
            try
            {
                using (MySqlConnection c = new MySqlConnection(ConnectionStringBuilder.ToString()))
                {
                    c.Open();
                    job.QueryJob(c);
                }
            }
            catch (Exception ex)
            {
                if (job.ExceptionJob != null)
                    job.ExceptionJob(ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed == true)
            {
                return;
            }

            if (disposing == true)
            {
                Clear();
            }
            disposed = true;
        }

        private void Clear()
        {
            MySqlConnection.ClearAllPools();
        }

        private bool disposed = false;
        private readonly MySqlConnectionStringBuilder ConnectionStringBuilder;

    }
}
