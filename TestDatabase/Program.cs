using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.Database;
using Laika.Database.MySql;
using Laika.Database.SqlServer;
using Laika.Database.Sharding;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
namespace TestDatabase
{
    internal class ShardDB : DatabaseShardingBase<string, string>
    {
        public override IDatabase FindDB(string key)
        {
            if (AllDataBase.ContainsKey(key) == false)
                return null;

            return AllDataBase[key];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IDatabase db = TestMySqlDatabase();
            TestSharding(db);

            TestSqlServerDataBase();
        }

        private static void TestSharding(IDatabase db)
        {
            IDatabaseSharding<string, string> ShardingDB = new ShardDB();
            ShardingDB.AddDatabase("mysqlDB1", db);
            IDatabase findDB = ShardingDB.FindDB("mysqlDB1");
            ShardingDB.Dispose();
        }

        private static void TestSqlServerDataBase()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = @"SWKWON-PC\SQLEXPRESS";
            sb.InitialCatalog = "myGame";
            sb.IntegratedSecurity = true;
            sb.UserID = @"NEXON\swkwon";
            sb.Pooling = true;
            sb.MinPoolSize = 10;
            sb.MaxPoolSize = 10;
            IDatabase db = CreateDbFactory.CreateSqlServerDatabase(sb);

            TestAsyncNormalSqlJob(db);
            TestAsyncTransactionSqlJob(db);
            TestRollbackAsyncTransactionSqlJob(db);
        }

        private static void TestRollbackAsyncTransactionSqlJob(IDatabase db)
        {
            SqlServerDbJob job = SqlServerDbJob.CreateSqlServerTransactionJob(transactionContext =>
            {
                using (SqlCommand cmd = transactionContext.CreateDbCommand("INSERT INTO test(col) VALUES(2);"))
                {
                    cmd.ExecuteNonQuery();
                }

                // if want rollback
                transactionContext.NeedRollback = true;
            },
            ex =>
            {
                // if caught exception auto rollback
                Console.WriteLine(ex.ToString());
            });

            db.DoJob(job);
        }

        private static void TestAsyncTransactionSqlJob(IDatabase db)
        {
            SqlServerDbJob job = SqlServerDbJob.CreateSqlServerTransactionJob(transactionContext =>
            {
                using (SqlCommand cmd = transactionContext.CreateDbCommand("INSERT INTO test(col) VALUES(1);"))
                {
                    cmd.ExecuteNonQuery();
                }
            },
            ex =>
            {
                Console.WriteLine(ex.ToString());
            });

            Task jobTask = db.AsyncDoJob(job);
            jobTask.Wait();
        }

        private static void TestAsyncNormalSqlJob(IDatabase db)
        {
            SqlServerDbJob job = SqlServerDbJob.CreateSqlServerJob(connection =>
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO test(col) VALUES(1);", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }, ex =>
            {
                Console.WriteLine(ex.ToString());
            });

            Task jobTask = db.AsyncDoJob(job);
            jobTask.Wait();
        }

        private static IDatabase TestMySqlDatabase()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = "localhost";
            sb.Port = 3306;
            sb.UserID = "root";
            sb.Password = "1234";
            sb.Database = "test";
            sb.Pooling = true;
            sb.MinimumPoolSize = 10;
            sb.MaximumPoolSize = 20;
            
            IDatabase db = CreateDbFactory.CreateMySqlDatabase(sb);

            TestAsyncNormalJob(db);
            TestSyncNormalJob(db);
            TestAsyncTransactionJob(db);
            TestSyncTransactionJob(db);
            TestRollbackTransactionJob(db);
            TestManualCommitTransaction(db);

            return db;
        }

        private static void TestManualCommitTransaction(IDatabase db)
        {
            MySqlDbJob job = MySqlDbJob.CreateMySqlTransactionJob(transactionContext => 
            {
                transactionContext.ManualCommit = true;
                using (MySqlCommand cmd = transactionContext.CreateDbCommand("INSERT INTO test(col) VALUES(10);"))
                {
                    cmd.ExecuteNonQuery();
                }
            });

            MySqlDbJob job1 = MySqlDbJob.CreateMySqlTransactionJob(transactionContext =>
            {
                transactionContext.ManualCommit = true;
                using (MySqlCommand cmd = transactionContext.CreateDbCommand("INSERT INTO test(col) VALUES(21);"))
                {
                    cmd.ExecuteNonQuery();
                }
            });

            db.DoJob(job);
            db.DoJob(job1);

            if (job.ReadyToCommit == true && job1.ReadyToCommit == true)
            {
                job.Commit();
                job1.Commit();
            }
            else
            {
                job.Rollback();
                job1.Rollback();
            }
        }

        private static void TestRollbackTransactionJob(IDatabase db)
        {
            MySqlDbJob job = MySqlDbJob.CreateMySqlTransactionJob(transactionContext =>
            {
                using (MySqlCommand cmd = transactionContext.CreateDbCommand("INSERT INTO test(col) VALUES(1);"))
                {
                    cmd.ExecuteNonQuery();
                }

                // if want rollback
                transactionContext.NeedRollback = true;
            },
            ex =>
            {
                // if caught exception auto rollback
                Console.WriteLine(ex.ToString());
            });

            db.DoJob(job);
        }

        private static void TestSyncTransactionJob(IDatabase db)
        {
            MySqlDbJob job = MySqlDbJob.CreateMySqlTransactionJob(transactionContext =>
            {
                using (MySqlCommand cmd = transactionContext.CreateDbCommand("INSERT INTO test(col) VALUES(1);"))
                {
                    cmd.ExecuteNonQuery();
                }
            },
            ex =>
            {
                Console.WriteLine(ex.ToString());
            });

            db.DoJob(job);
        }

        private static void TestAsyncTransactionJob(IDatabase db)
        {
            MySqlDbJob job = MySqlDbJob.CreateMySqlTransactionJob(transactionContext => 
            {
                using (MySqlCommand cmd = transactionContext.CreateDbCommand("INSERT INTO test(col) VALUES(1);"))
                {
                    cmd.ExecuteNonQuery();
                }
            }, 
            ex => 
            {
                Console.WriteLine(ex.ToString());
            });

            Task jobTask = db.AsyncDoJob(job);
            jobTask.Wait();
        }

        private static void TestSyncNormalJob(IDatabase db)
        {
            MySqlDbJob job = MySqlDbJob.CreateMySqlJob(connection =>
            {
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO test(col) VALUES(1);", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }, ex =>
            {
                Console.WriteLine(ex.ToString());
            });

            db.DoJob(job);
        }

        private static void TestAsyncNormalJob(IDatabase db)
        {
            MySqlDbJob job = MySqlDbJob.CreateMySqlJob(connection => 
            {
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO test(col) VALUES(1);", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }, ex => 
            {
                Console.WriteLine(ex.ToString());
            });

            Task jobTask = db.AsyncDoJob(job);
            jobTask.Wait();
        }
    }
}
