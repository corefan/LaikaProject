﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Laika.Database.MySql
{
    /// <summary>
    /// MySql 데이터베이스 클래스
    /// </summary>
    public class DatabaseForMySql : DatabaseBase, IDisposable
    {
        private bool disposed = false;

        internal DatabaseForMySql(MySqlConnectionStringBuilder connection)
            : this(connection.ToString())
        {
            
        }

        internal DatabaseForMySql(string connectionString)
            : base(connectionString)
        {
            InitializeConnection();
        }

        ~DatabaseForMySql()
        {
            Dispose(false);
        }
        /// <summary>
        /// Dispose 패턴
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override DbConnection GetConnection()
        {
            MySqlConnection c = new MySqlConnection(_connectionString);
            c.Open();
            return c;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed == true)
                return;
            if (disposing == true)
                Clear();

            disposed = true;
        }

        private void Clear()
        {
            MySqlConnection.ClearAllPools();
        }

        private void InitializeConnection()
        {
            MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            conn.Dispose();
            MySqlConnection.ClearAllPools();
        }
    }
}
