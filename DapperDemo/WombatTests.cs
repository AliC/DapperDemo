using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    public class WombatTests : TestSetup
    {
        public WombatTests()
        {
            _databaseName = "DapperDemo";
            _createTableScript = "CREATE TABLE dbo.Wombat ( WombatId INT IDENTITY PRIMARY KEY, Name NVARCHAR(128), GeographicalAddress NVARCHAR(256), Rating INT )";
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
        public void GivenFixedDBColumnNames_WhenIWantToUseDifferentPropertyNamesInDTOs_ThenMappingWorksCorrectly()
        {
            IList<Wombat> expectedWombats = TestData.GetWombats();
            IList<Wombat> actualWombats;

            string createWombatScript = TestData.GetInsertScriptFor(expectedWombats);
            ExecuteSqlForDatabase(createWombatScript);

            SqlMapper.SetTypeMap(typeof(Wombat), new ColumnAttributeTypeMapper<Wombat>());

            using (IDbConnection connection = new SqlConnection(GetConnectionString(_databaseName)))
            {
                connection.Open();

                actualWombats = connection.Query<Wombat>("SELECT * FROM dbo.Wombat").ToList();
            }

            Assert.That(actualWombats.Count, Is.EqualTo(4));

            Assert.That(actualWombats[0].Id, Is.EqualTo(expectedWombats[0].Id));
            Assert.That(actualWombats[0].Name, Is.EqualTo(expectedWombats[0].Name));
            Assert.That(actualWombats[0].Address, Is.EqualTo(expectedWombats[0].Address));
            Assert.That(actualWombats[0].Cuteness, Is.EqualTo(expectedWombats[0].Cuteness));

            Assert.That(actualWombats[1].Id, Is.EqualTo(expectedWombats[1].Id));
            Assert.That(actualWombats[1].Name, Is.EqualTo(expectedWombats[1].Name));
            Assert.That(actualWombats[1].Address, Is.EqualTo(expectedWombats[1].Address));
            Assert.That(actualWombats[1].Cuteness, Is.EqualTo(expectedWombats[1].Cuteness));
        }


    }
}
