using System.Collections.Generic;
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

        [Test]
        public void GivenClassAssociation_WhenDataReadFromDatabase_DataMappedToMultipleObjects()
        {
            IList<Dodo> expectedDodos = TestData.GetDodos();
            IList<Dodo> actualDodos;

            string createDodoScript = TestData.GetInsertScriptFor(expectedDodos);
        }
    }
}
