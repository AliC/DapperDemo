using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class DapperIntegrationTests
    {
        private string _databaseName = "DapperDemo";
        private string _createDogTableScript = "CREATE TABLE dbo.Dog ( DogId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128), Breed NVARCHAR(64), Age INT )";

        [SetUp]
        public void SetUp()
        {
            CreateDatabase();
            ExecuteSqlForDatabase(_createDogTableScript);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteDatabase();
        }

        [Test]
        public void GivenNoDogs_WhenCreatingDog_ThenDogCreated()
        {
            Dog expectedDog = new Dog { Name = "Fido", Breed = "mongrel", Age = 2 };
            List<Dog> actualDogs = new List<Dog>();

            SqlConnectionStringBuilder builder = GetBaseConnectionString();
            builder.InitialCatalog = _databaseName;

            using (IDbConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                connection.Execute("INSERT dbo.Dog SELECT @Name, @Breed, @Age", new { expectedDog.Name, expectedDog.Breed, expectedDog.Age });

                using (IDbCommand dbCommand = connection.CreateCommand())
                {
                    dbCommand.CommandText = "SELECT * FROM dbo.Dog";
                    using (IDataReader reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            actualDogs.Add(new Dog { Name = (string)reader["Name"], Breed = (string)reader["Breed"], Age = (int)reader["Age"], });
                        }
                    }
                }
            }

            Assert.That(actualDogs.Count, Is.EqualTo(1));
            Assert.That(actualDogs[0].Name, Is.EqualTo(expectedDog.Name));
            Assert.That(actualDogs[0].Breed, Is.EqualTo(expectedDog.Breed));
            Assert.That(actualDogs[0].Age, Is.EqualTo(expectedDog.Age));
        }

        [Test]
        public void GivenTwoDogs_WhenReadingDogs_ThenDogsRead()
        {
            IList<Dog> expectedDogs = new List<Dog>
            {
                new Dog { Name = "Fido", Breed = "mongrel", Age = 2 },
                new Dog { Name = "Shep", Breed = "sheepdog", Age = 14 }
            };
            List<Dog> actualDogs = new List<Dog>();

            string createTwoDogsScript =
                "INSERT dbo.Dog SELECT " +
                $"'{expectedDogs[0].Name}', '{expectedDogs[0].Breed}', {expectedDogs[0].Age} UNION SELECT " +
                $"'{expectedDogs[1].Name}', '{expectedDogs[1].Breed}', {expectedDogs[1].Age}";

            ExecuteSqlForDatabase(createTwoDogsScript);

            SqlConnectionStringBuilder builder = GetBaseConnectionString();
            builder.InitialCatalog = _databaseName;

            using (IDbConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                actualDogs = connection.Query<Dog>("SELECT * FROM dbo.Dog").ToList();
            }

            Assert.That(actualDogs.Count, Is.EqualTo(2));

            Assert.That(actualDogs[0].Name, Is.EqualTo(expectedDogs[0].Name));
            Assert.That(actualDogs[0].Breed, Is.EqualTo(expectedDogs[0].Breed));
            Assert.That(actualDogs[0].Age, Is.EqualTo(expectedDogs[0].Age));

            Assert.That(actualDogs[1].Name, Is.EqualTo(expectedDogs[1].Name));
            Assert.That(actualDogs[1].Breed, Is.EqualTo(expectedDogs[1].Breed));
            Assert.That(actualDogs[1].Age, Is.EqualTo(expectedDogs[1].Age));
        }

        [Ignore("Implement test - read by an attribute (maybe two tests, one for id and one for name)")]
        public void Foo()
        {

        }

        private void CreateDatabase()
        {
            SqlConnectionStringBuilder connectionStringBuilder = GetBaseConnectionString();

            ExecuteSql(connectionStringBuilder.ConnectionString, $"CREATE DATABASE {_databaseName}");
        }

        private void DeleteDatabase()
        {
            SqlConnectionStringBuilder connectionStringBuilder = GetBaseConnectionString();
            
            ExecuteSql(connectionStringBuilder.ConnectionString, $"DROP DATABASE {_databaseName}");
        }

        private void ExecuteSqlForDatabase(string sqlScript)
        {
            SqlConnectionStringBuilder connectionStringBuilder = GetBaseConnectionString();
            connectionStringBuilder.InitialCatalog = _databaseName;

            ExecuteSql(connectionStringBuilder.ConnectionString, sqlScript);
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
        private static SqlConnectionStringBuilder GetBaseConnectionString()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = "(LocalDB)\\MSSQLLocalDB";
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.ConnectTimeout = 30;

            connectionStringBuilder.Pooling = false;

            return connectionStringBuilder;
        }
    }
}
