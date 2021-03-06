﻿using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class TestSetup
    {
        private bool _dontExecuteSql = true;

        protected string _databaseName = "DapperDemo";

        protected string _createTableScript;

        [SetUp]
        public virtual void SetUp()
        {
            CreateDatabase();
            ExecuteSqlForDatabase(_createTableScript);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteDatabase();
        }

        protected void CreateDatabase()
        {
            ExecuteSql(GetConnectionString(), $"CREATE DATABASE {_databaseName}");
        }

        protected void DeleteDatabase()
        {
            ExecuteSql(GetConnectionString(), $"DROP DATABASE {_databaseName}");
        }

        protected void ExecuteSqlForDatabase(string sqlScript)
        {
            ExecuteSql(GetConnectionString(_databaseName), sqlScript);
        }

        protected static string GetConnectionString(string databaseName = null)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = "(LocalDB)\\MSSQLLocalDB";
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.ConnectTimeout = 30;
            connectionStringBuilder.InitialCatalog = databaseName ?? "master";

            connectionStringBuilder.Pooling = false;

            return connectionStringBuilder.ConnectionString;
        }

        private void ExecuteSql(string connectionString, string sqlScript)
        {
            if (_dontExecuteSql)
            {
                ExecuteSqlNoOp(connectionString, sqlScript);
            }
            else
            {
                ExecuteSqlOp(connectionString, sqlScript);
            }
        }

        private void ExecuteSqlNoOp(string connectionString, string sqlScript)
        {
            Console.WriteLine($"Executing SQL (no-op): {sqlScript}\n");
        }

        private void ExecuteSqlOp(string connectionString, string sqlScript)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Server server = new Server(new ServerConnection(connection));

                Console.WriteLine($"Executing SQL: {sqlScript}\n");

                server.ConnectionContext.ExecuteNonQuery(sqlScript);
            }
        }
    }
}