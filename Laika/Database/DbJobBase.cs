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
        /// <summary>
        /// 수동 commit일 경우 commit 준비가 되면 필드는 true가 됩니다. 
        /// transaction 실패 시 false가 됩니다.
        /// </summary>
        public bool ReadyToCommit { get; private set; }
        
        protected abstract void TransactionContextDispose();

        protected Action<MySqlConnection> _mySqlJob = null;
        protected Action<MySqlTransactionContext> _mySqlTransactionJob = null;

        protected Action<SqlConnection> _sqlServerJob = null;
        protected Action<SqlServerTransactionContext> _sqlServerTransactionJob = null;

        protected Action<Exception> _exception = null;

        protected TransactionContextBase _transaction = null;
        private bool _failed = false;
        /// <summary>
        /// 수동 commit을 할 경우 transaction 성공 시 commit됩니다.
        /// </summary>
        public void Commit()
        {
            if (_transaction == null)
                return;

            if (_transaction.ManualCommit == true && _transaction.NeedRollback == false)
                _transaction.Commit();
            else
                _transaction.Rollback();

            TransactionContextDispose();
        }
        /// <summary>
        /// 수동 commit을 할 경우 transaction rollback을 합니다.
        /// </summary>
        public void Rollback()
        {
            if (_transaction == null)
                return;

            _transaction.Rollback();

            TransactionContextDispose();
        }

        internal void DoJob(DbConnection connection)
        {
            try
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
                else
                {
                    Exception e = new Exception("Invalid Job.");
                    _exception(e);
                    if (_transaction != null)
                    {
                        _transaction.Rollback();
                        TransactionContextDispose();
                    }
                }
            }
            catch (Exception e)
            {
                _exception(e);
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
                _transaction = transaction;

                _sqlServerTransactionJob(transaction);
                if (transaction.ManualCommit == false)
                {
                    if (transaction.NeedRollback == true)
                        transaction.Rollback();
                    else
                        transaction.Commit();

                    _transaction = null;
                }
                else
                {
                    ReadyToCommit = true;
                }
            }
            catch (Exception e)
            {
                _failed = true;
                if (transaction != null)
                    transaction.Rollback();

                if (_exception != null)
                    _exception(e);
            }
            finally
            {
                if (transaction.ManualCommit == false || _failed == true)
                {
                    if (transaction != null)
                    {
                        transaction.Dispose();
                        _transaction = null;
                    }
                }
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
                _transaction = transaction;
                _mySqlTransactionJob(transaction);

                if (transaction.ManualCommit == false)
                {
                    if (transaction.NeedRollback == true)
                        transaction.Rollback();
                    else
                        transaction.Commit();

                    _transaction = null;
                }
                else
                {
                    ReadyToCommit = true;
                }
            }
            catch (Exception e)
            {
                _failed = true;
                if (transaction != null)
                    transaction.Rollback();

                if (_exception != null)
                    _exception(e);
            }
            finally
            {
                if (transaction.ManualCommit == false || _failed == true)
                {
                    if (transaction != null)
                    {
                        transaction.Dispose();
                        _transaction = null;
                    }
                }
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
