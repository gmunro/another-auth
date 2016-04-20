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
            IUserManager userManager = new UserManager(authDb);

            const string primaryEmail = "garethmu @gmail.com";
            userManager.CreateUser(primaryEmail);
            Assert.IsTrue(tAuthDb.SaveCalled, "Save was not called on db");
            Assert.IsTrue(userManager.UserExistsByEmail(primaryEmail),"Newly registered user did not exist.");

            IUserManager otherUserManager = new UserManager(authDb);
            Assert.IsTrue(otherUserManager.UserExistsByEmail(primaryEmail),"New UserManager backed by same db, user did not exist.");
        }

        [TestMethod]
        public void StandardLoginPersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;
            IUserManager userManager = new UserManager(authDb);

            const string primaryEmail = "garethmu@gmail.com";
            var user = userManager.CreateUser(primaryEmail);
            Assert.IsTrue(tAuthDb.SaveCalled);

            

            ILoginManager loginManager = new StandardLoginManager(authDb);
            const string password = "zzz1";
            loginManager.CreateLogin(user, password);
            Assert.IsTrue(tAuthDb.SaveCalled);
            Assert.IsTrue(loginManager.LoginExists(user),"Login exists returned false for newly created login");

            ILoginManager otherLoginManager = new StandardLoginManager(authDb);
            Assert.IsTrue(otherLoginManager.LoginExists(user), "Login did not persist through new LoginManager");
        }
    }
}
