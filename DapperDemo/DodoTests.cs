using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    public class DodoTests : TestSetup
    {
        public DodoTests()
        {
            _createTableScript = "CREATE TABLE dbo.Dodo ( Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(128))";
            _createTableScript += "\nGO\nCREATE TABLE dbo.GameStatistics (Id INT IDENTITY PRIMARY KEY, DodoId INT REFERENCES Dodo(Id), Bitepower INT, Cuteness INT, Speed INT)";
        }

        [Ignore("complete test")]
        public void GivenClassAssociation_WhenDataReadFromDatabase_DataMappedToMultipleObjects()
        {
            IList<Dodo> expectedDodos = TestData.GetDodos();
            IList<Dodo> actualDodos;

            string createDodoScript = TestData.GetInsertScriptFor(expectedDodos);

            ExecuteSqlForDatabase(createDodoScript);

            using (IDbConnection connection = new SqlConnection(GetConnectionString(_databaseName)))
            {
                connection.Open();

                actualDodos = connection.Query<Dodo>("SELECT * FROM dbo.Wombat").ToList();
            }

            Assert.That(actualDodos.Count, Is.EqualTo(4));

            Assert.That(actualDodos[0].Id, Is.EqualTo(expectedDodos[0].Id));
            Assert.That(actualDodos[0].Name, Is.EqualTo(expectedDodos[0].Name));
            Assert.That(actualDodos[0].GameStatistics.BitePower, Is.EqualTo(expectedDodos[0].GameStatistics.BitePower));

            Assert.That(actualDodos[1].Id, Is.EqualTo(expectedDodos[1].Id));
            Assert.That(actualDodos[1].Name, Is.EqualTo(expectedDodos[1].Name));
        }
    }
}
