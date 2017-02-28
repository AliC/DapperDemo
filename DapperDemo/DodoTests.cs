using System.Collections.Generic;
using NUnit.Framework;

namespace DapperDemo
{
    public class DodoTests : TestSetup
    {
        public DodoTests()
        {
            _createTableScript = "CREATE TABLE dbo.Dodo ( Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(128))";
        }

        [Test]
        public void GivenClassAssociation_WhenDataReadFromDatabase_DataMappedToMultipleObjects()
        {
            IList<Dodo> expectedDodos = TestData.GetDodos();
        }
    }
}
