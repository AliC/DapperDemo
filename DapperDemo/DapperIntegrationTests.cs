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
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=c:\codemy\DapperDemo\DapperDemo\DapperDemo.mdf;Integrated Security=True;Connect Timeout=30";
        string createDogTableScript = "CREATE TABLE dbo.Dog ( DogId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128), Breed NVARCHAR(64), Age INT )";
        string deleteDogTableScript = "DROP TABLE dbo.Dog";

        [SetUp]
        public void SetUp()
        {
            ExecuteSql(createDogTableScript);
        }

        [TearDown]
        public void TearDown()
        {
            ExecuteSql(deleteDogTableScript);
        }

        [Test]
        public void GivenNoDogs_WhenCreatingDog_ThenDogCreated()
        {
            Dog expectedDog = new Dog { Name = "Fido", Breed = "mongrel", Age = 2 };
            List<Dog> actualDogs = new List<Dog>();

            using (IDbConnection connection = new SqlConnection(connectionString))
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

            using (IDbConnection connection = new SqlConnection(connectionString))
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

        private void ExecuteSql(string sqlScript)
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
