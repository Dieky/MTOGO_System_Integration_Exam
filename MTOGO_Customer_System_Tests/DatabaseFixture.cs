using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTOGO_Customer_System_Tests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            // Recreate the database only once before the tests are executed
            DatabaseIntegrationTestHelper.RecreateDatabase();
        }

        public void Dispose()
        {
            // Optionally clean up the database after all tests if needed. Nothing needs to be done at the moment
        }
    }

}
