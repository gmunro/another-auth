using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace another_auth.tests
{
    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void RegisterPersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu @gmail.com";

            IAuthManager authManager = new AuthManager(authDb);

            authManager.RegisterUser(primaryEmail);

            Assert.IsTrue(tAuthDb.SaveCalled, "Save was not called on db");

            Assert.IsTrue(authManager.UserExistsByEmail(primaryEmail),"Newly registered user did not exist.");

            IAuthManager otherAuthManager = new AuthManager(authDb);

            Assert.IsTrue(otherAuthManager.UserExistsByEmail(primaryEmail),"New auth manager backed by same db, user did not exist.");
        }
    }
}
