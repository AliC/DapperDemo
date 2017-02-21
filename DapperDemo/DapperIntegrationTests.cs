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
        private string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30";
        private string _databaseName = "DapperDemo";
        private string _createDogTableScript = "CREATE TABLE dbo.Dog ( DogId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128), Breed NVARCHAR(64), Age INT )";

        [SetUp]
        public void SetUp()
        {
            ExecuteSql($"CREATE DATABASE {_databaseName}");
            ExecuteSqlForDB(_createDogTableScript);
        }

        [TearDown]
        public void TearDown()
        {
            ExecuteSql("DROP DATABASE DapperDemo");
        }

        [Test]
        public void GivenNoDogs_WhenCreatingDog_ThenDogCreated()
        {
            Dog expectedDog = new Dog { Name = "Fido", Breed = "mongrel", Age = 2 };
            List<Dog> actualDogs = new List<Dog>();

            using (IDbConnection connection = new SqlConnection(GetConnectionStringForDB()))
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

            ExecuteSql(createTwoDogsScript);

            using (IDbConnection connection = new SqlConnection(GetConnectionStringForDB()))
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

        private void ExecuteSqlForDB(string sqlScript)
        {
            ExecuteSql(sqlScript, GetConnectionStringForDB());
        }

        private void ExecuteSql(string sqlScript, string connectionString = null)
        {
            if (connectionString == null)
            {
                connectionString = _connectionString;
            }

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

        private string GetConnectionStringForDB()
        {
            return _connectionString += $";Initial Catalog={_databaseName}";
        }
    }
}
