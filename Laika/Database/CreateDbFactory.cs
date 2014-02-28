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
    /// <summary>
    /// Database 인스턴스 팩토리
    /// </summary>
    public static class CreateDbFactory
    {
        /// <summary>
        /// MySql Database 인스턴스 생성
        /// </summary>
        /// <param name="conn">Connection 빌더</param>
        /// <returns>Database 인터페이스</returns>
        public static IDatabase CreateMySqlDatabase(MySqlConnectionStringBuilder conn)
        {
            return new DatabaseForMySql(conn);
        }
        /// <summary>
        /// MySql Database 인스턴스 생성
        /// </summary>
        /// <param name="connectionString">Connection 스트링</param>
        /// <returns>Database 인터페이스</returns>
        public static IDatabase CreateMySqlDatabase(string connectionString)
        {
            return new DatabaseForMySql(connectionString);
        }
        /// <summary>
        /// SqlServer Database 인스턴스 생성
        /// </summary>
        /// <param name="conn">Connection 빌더</param>
        /// <returns>Database 인터페이스</returns>
        public static IDatabase CreateSqlServerDatabase(SqlConnectionStringBuilder conn)
        {
            return new DatabaseForSqlServer(conn);
        }
        /// <summary>
        /// SqlServer Database 인스턴스 생성
        /// </summary>
        /// <param name="connectionString">Connection 스트링</param>
        /// <returns>Database 인터페이스</returns>
        public static IDatabase CreateSqlServerDatabase(string connectionString)
        {
            return new DatabaseForSqlServer(connectionString);
        }
    }
}
