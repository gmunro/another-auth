using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace another_auth.tests
{
    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void UserPersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu @gmail.com";

            IAuthManager authManager = new AuthManager(authDb);

            authManager.CreateUser(primaryEmail);

            Assert.IsTrue(tAuthDb.SaveCalled, "Save was not called on db");

            Assert.IsTrue(authManager.UserExistsByEmail(primaryEmail),"Newly registered user did not exist.");

            IAuthManager otherAuthManager = new AuthManager(authDb);

            Assert.IsTrue(otherAuthManager.UserExistsByEmail(primaryEmail),"New auth manager backed by same db, user did not exist.");
        }

        [TestMethod]
        public void LoginPersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            IAuthManager authManager = new AuthManager(authDb);

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = authManager.CreateUser(primaryEmail);

            authManager.CreateLogin(user, AuthManager.LoginType.Standard, password);

            Assert.IsTrue(tAuthDb.SaveCalled);

            IAuthManager otherAuthManager = new AuthManager(authDb);

            Assert.IsTrue(otherAuthManager.LoginExists(user, AuthManager.LoginType.Standard));
        }
    }
}
