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
            _createTableScript = "CREATE TABLE dbo.Dodo ( DodoId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128))";
            _createTableScript += "\nGO\nCREATE TABLE dbo.GameStatistics (Id INT IDENTITY PRIMARY KEY, DodoId INT REFERENCES Dodo(DodoId), Bitepower INT, Cuteness INT, Speed INT)";
        }

        [Test]
        public void GivenClassAssociation_WhenDataReadFromDatabase_DataMappedToMultipleObjects()
        {
            IList<Dodo> expectedDodos = TestData.GetDodos();
            IList<Dodo> actualDodos;

            string createDodoScript = TestData.GetInsertScriptFor(expectedDodos);

            ExecuteSqlForDatabase(createDodoScript);

            SqlMapper.SetTypeMap(typeof(Dodo), new ColumnAttributeTypeMapper<Dodo>());

            using (IDbConnection connection = new SqlConnection(GetConnectionString(_databaseName)))
            {
                connection.Open();

                actualDodos = connection.Query<Dodo, GameStatistics, Dodo>(
                    "SELECT * FROM dbo.Dodo d LEFT OUTER JOIN dbo.GameStatistics g ON d.DodoId = g.DodoId",
                    (dodo, stat) =>
                    {
                        dodo.GameStatistics = stat;

                        return dodo;
                    }).ToList();
            }

            Assert.That(actualDodos.Count, Is.EqualTo(4));

            for (int i = 0; i < 4; i++)
            {
                Assert.That(actualDodos[i].Id, Is.EqualTo(expectedDodos[i].Id));
                Assert.That(actualDodos[i].Name, Is.EqualTo(expectedDodos[i].Name));
                Assert.That(actualDodos[i].GameStatistics.BitePower, Is.EqualTo(expectedDodos[i].GameStatistics.BitePower));
                Assert.That(actualDodos[i].GameStatistics.Cuteness, Is.EqualTo(expectedDodos[i].GameStatistics.Cuteness));
                Assert.That(actualDodos[i].GameStatistics.Speed, Is.EqualTo(expectedDodos[i].GameStatistics.Speed));
            }
        }
    }
}
