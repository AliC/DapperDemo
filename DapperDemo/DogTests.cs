using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class DogTests : TestSetup
    {

        public DogTests()
        {
            _createTableScript = "CREATE TABLE dbo.Dog ( DogId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128), Breed NVARCHAR(64), Age INT )";
            _databaseName = "DapperDemo";
        }

        [SetUp]
        public void SetUp()
        {
            CreateDatabase();
            ExecuteSqlForDatabase(_createTableScript);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteDatabase();
        }

        [Test]
        public void GivenDogs_WhenFilteringById_CorrectDogReturned()
        {
            IList<Dog> startingDogs = TestData.GetDogs();

            int expectedId = 2;
            Dog expectedDog = TestData.GetDogs(expectedId);

            List<Dog> actualDogs;

            string createDogScript = TestData.GetInsertScriptFor(startingDogs);
            ExecuteSqlForDatabase(createDogScript);

            using (IDbConnection connection = new SqlConnection(GetConnectionString(_databaseName)))
            {
                connection.Open();

                actualDogs = connection.Query<Dog>("SELECT * FROM dbo.Dog WHERE DogId = @DogId", new { DogId = expectedId }).ToList();
            }

            Assert.That(actualDogs.Count, Is.EqualTo(1));

            Assert.That(actualDogs[0].DogId, Is.EqualTo(expectedDog.DogId));
            Assert.That(actualDogs[0].Name, Is.EqualTo(expectedDog.Name));
            Assert.That(actualDogs[0].Breed, Is.EqualTo(expectedDog.Breed));
            Assert.That(actualDogs[0].Age, Is.EqualTo(expectedDog.Age));
        }

        [Test]
        public void GivenDogs_WhenFilteringByName_CorrectDogsReturned()
        {
            IList<Dog> startingDogs = TestData.GetDogs();

            string expectedName = "Lassie";
            IList<Dog> expectedDogs = TestData.GetDogs(expectedName);

            List<Dog> actualDogs;

            string createDogScript = TestData.GetInsertScriptFor(startingDogs);
            ExecuteSqlForDatabase(createDogScript);

            using (IDbConnection connection = new SqlConnection(GetConnectionString(_databaseName)))
            {
                connection.Open();

                actualDogs = connection.Query<Dog>("SELECT * FROM dbo.Dog WHERE Name = @Name", new { Name = expectedName }).ToList();
            }

            Assert.That(actualDogs.Count, Is.EqualTo(2));

            Assert.That(actualDogs[0].DogId, Is.EqualTo(expectedDogs[0].DogId));
            Assert.That(actualDogs[0].Name, Is.EqualTo(expectedDogs[0].Name));
            Assert.That(actualDogs[0].Breed, Is.EqualTo(expectedDogs[0].Breed));
            Assert.That(actualDogs[0].Age, Is.EqualTo(expectedDogs[0].Age));

            Assert.That(actualDogs[1].DogId, Is.EqualTo(expectedDogs[1].DogId));
            Assert.That(actualDogs[1].Name, Is.EqualTo(expectedDogs[1].Name));
            Assert.That(actualDogs[1].Breed, Is.EqualTo(expectedDogs[1].Breed));
            Assert.That(actualDogs[1].Age, Is.EqualTo(expectedDogs[1].Age));
        }

        [Test]
        public void GivenNoDogs_WhenCreatingDog_ThenDogCreated()
        {
            Dog expectedDog = TestData.GetDogs(1);
            List<Dog> actualDogs = new List<Dog>();

            using (IDbConnection connection = new SqlConnection(GetConnectionString(_databaseName)))
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
        public void GivenDogs_WhenReadingDogs_ThenDogsRead()
        {
            IList<Dog> expectedDogs = TestData.GetDogs();
            List<Dog> actualDogs;

            string createDogScript = TestData.GetInsertScriptFor(expectedDogs);
            ExecuteSqlForDatabase(createDogScript);

            using (IDbConnection connection = new SqlConnection(GetConnectionString(_databaseName)))
            {
                connection.Open();

                actualDogs = connection.Query<Dog>("SELECT * FROM dbo.Dog").ToList();
            }

            Assert.That(actualDogs.Count, Is.EqualTo(4));

            Assert.That(actualDogs[0].Name, Is.EqualTo(expectedDogs[0].Name));
            Assert.That(actualDogs[0].Breed, Is.EqualTo(expectedDogs[0].Breed));
            Assert.That(actualDogs[0].Age, Is.EqualTo(expectedDogs[0].Age));

            Assert.That(actualDogs[1].Name, Is.EqualTo(expectedDogs[1].Name));
            Assert.That(actualDogs[1].Breed, Is.EqualTo(expectedDogs[1].Breed));
            Assert.That(actualDogs[1].Age, Is.EqualTo(expectedDogs[1].Age));
        }
    }
}
