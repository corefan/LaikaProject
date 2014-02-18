using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySqlDB
{
    public class DatabaseForMySql : IDatabase
    {
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
        }

        private void InitializeConnection()
        {
            MySqlConnection conn = new MySqlConnection(ConnectionStringBuilder.ToString());
            conn.Open();
            conn.Dispose();
        }

        public Task DoJob(IDBJob job)
        {
            if (job.QueryJob != null)
            {
                return NormalQueryJob(job);
            }

            if (job.TransactionQueryJob != null)
            {
                return TransactionQueryJob(job);
            }
            
            return null;
        }

        private Task TransactionQueryJob(IDBJob job)
        {
            Task result = Task.Factory.StartNew(() =>
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
            });

            return result;
        }

        private Task NormalQueryJob(IDBJob job)
        {
            Task result = Task.Factory.StartNew(() =>
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
            });
            return result;
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
