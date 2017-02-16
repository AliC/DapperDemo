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

        [Test]
        public void GivenNoDogs_WhenCreatingDog_ThenDogCreated()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                Dog expectedDog = new Dog { Name = "Fido", Breed = "mongrel", Age = 2 };

                connection.Open();

                connection.Execute("CREATE TABLE dbo.Dog ( DogId INT PRIMARY KEY, Name NVARCHAR(128), Breed NVARCHAR(64), Age INT )");

                connection.Execute("INSERT dbo.Dog SELECT @Name, @Breed, @Age", new { expectedDog.Name, expectedDog.Breed, expectedDog.Age});

                IDbCommand dbCommand = connection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM dbo.Dog";

                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {

                    }
                }
            }


        }
    }
}
