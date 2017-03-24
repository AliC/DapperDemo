using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    public class AardvarkTests : TestSetup
    {
        // use stored proc for retrieval

        public void GivenClassAssociation_WhenDataReadFromDatabaseWithConventionalIdColumns_DataMappedToMultipleObjects()
        {
            IList<Aardvark> expectedAardvarks = TestData.GetAardvarks();
            IList<Aardvark> actualAardvarks;

            // use conventional names for Id columns

            using (IDbConnection connection = new SqlConnection("TODO"))
            {
                connection.Open();

                actualAardvarks = connection.Query<Aardvark>(
                    "dbo.GetAardvarks",
                    commandType: CommandType.StoredProcedure).ToList();
            }

            Assert.That(actualAardvarks.Count, Is.EqualTo(4));

            for (int i = 0; i < 4; i++)
            {
                Assert.That(actualAardvarks[i].Id, Is.EqualTo(expectedAardvarks[i].Id));
                Assert.That(actualAardvarks[i].Name, Is.EqualTo(expectedAardvarks[i].Name));

                CollectionAssert.AreEqual(actualAardvarks[i].Behaviours, expectedAardvarks[i].Behaviours);
            }

        }

        public void GivenClassAssociation_WhenDataReadFromDatabaseWithUnconventionalIdColumns_DataMappedToMultipleObjects()
        {
            // use unconventional names for Id columns


        }
    }
}
