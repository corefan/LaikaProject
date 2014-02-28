using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Laika.Database.MySql;
using Laika.Database.SqlServer;

namespace Laika.Database
{
    public abstract class DbJobBase
    {
        protected Action<MySqlConnection> _mySqlJob = null;
        protected Action<MySqlTransactionContext> _mySqlTransactionJob = null;

        protected Action<SqlConnection> _sqlServerJob = null;
        protected Action<SqlServerTransactionContext> _sqlServerTransactionJob = null;

        protected Action<Exception> _exception = null;

        internal void DoJob(DbConnection connection)
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
                throw new ArgumentException("Invalid db connection.");
                        
            if (_mySqlJob != null)
            {
                MySqlJobAction(connection);
            }
            else if (_mySqlTransactionJob != null)
            {
                MySqlTransactionAction(connection);
            }
            else if (_sqlServerJob != null)
            {
                SqlServerJobAction(connection);
            }
            else if (_sqlServerTransactionJob != null)
            {
                SqlTransactionAction(connection);
            }
        }

        private void SqlTransactionAction(DbConnection connection)
        {
            SqlConnection conn = null;
            SqlServerTransactionContext transaction = null;
            try
            {
                conn = (SqlConnection)connection;
                transaction = new SqlServerTransactionContext(conn);

                _sqlServerTransactionJob(transaction);

                if (transaction.NeedRollback == true)
                    transaction.Rollback();
                else
                    transaction.Commit();
            }
            catch (Exception e)
            {
                if (transaction != null)
                    transaction.Rollback();

                if (_exception != null)
                    _exception(e);
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        private void SqlServerJobAction(DbConnection connection)
        {
            SqlConnection conn = null;
            try
            {
                conn = (SqlConnection)connection;
                _sqlServerJob(conn);
            }
            catch (Exception e)
            {
                if (_exception != null)
                    _exception(e);
            }
            finally
            {
                conn.Dispose();
            }
        }

        private void MySqlTransactionAction(DbConnection connection)
        {
            MySqlConnection conn = null;
            MySqlTransactionContext transaction = null;
            try
            {
                conn = (MySqlConnection)connection;
                transaction = new MySqlTransactionContext(conn);

                _mySqlTransactionJob(transaction);

                if (transaction.NeedRollback == true)
                    transaction.Rollback();
                else
                    transaction.Commit();
            }
            catch (Exception e)
            {
                if (transaction != null)
                    transaction.Rollback();

                if (_exception != null)
                    _exception(e);
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        private void MySqlJobAction(DbConnection connection)
        {
            MySqlConnection conn = null;
            try
            {
                conn = (MySqlConnection)connection;
                _mySqlJob(conn);
            }
            catch (Exception e)
            {
                if (_exception != null)
                    _exception(e);
            }
            finally
            {
                conn.Dispose();
            }
        }
    }
}
