using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;

namespace DapperDemo
{
    public class TestSetup
    {
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
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand dbCommand = connection.CreateCommand())
                {
                    dbCommand.CommandText = sqlScript;
                    dbCommand.ExecuteNonQuery();
                }
            }
        }
    }
}