using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace another_auth.tests
{
    [TestClass]
    public class RegisterAndLoginTests
    {
        private User CreateUserAccount(IAuthDb authDb, string primaryEmail)
        {
            IUserManager userManager = new UserManager(authDb);
            return userManager.CreateUser(primaryEmail);
        }

        private User CreateUserAccountWithStandardLogin(IAuthDb authDb, string primaryEmail, string password)
        {
            var user = CreateUserAccount(authDb, primaryEmail);
            ILoginManager loginManager = new StandardLoginManager(authDb);
            loginManager.CreateLogin(user, user.PrimaryEmailAddress, password);
            return user;
        }

        [TestMethod]
        public void UserPersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu @gmail.com";
            CreateUserAccount(authDb, primaryEmail);
            Assert.IsTrue(tAuthDb.SaveCalled, "Save was not called on db");

            IUserManager otherUserManager = new UserManager(authDb);
            Assert.IsTrue(otherUserManager.UserExistsByEmail(primaryEmail),"New UserManager backed by same db, user did not exist.");
        }

        [TestMethod]
        public void StandardLoginPersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = CreateUserAccountWithStandardLogin(authDb, primaryEmail, password);
            Assert.IsTrue(tAuthDb.SaveCalled);

            ILoginManager otherLoginManager = new StandardLoginManager(authDb);
            Assert.IsTrue(otherLoginManager.LoginExists(user), "LoginUsername did not persist through new LoginManager");
        }

        [TestMethod]
        public void LoginTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = CreateUserAccountWithStandardLogin(authDb, primaryEmail, password);
            Assert.IsNotNull(user);
            Assert.IsTrue(tAuthDb.SaveCalled);

            var loginManager = new StandardLoginManager(authDb);
            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.IsTrue(res.ResultType == LoginResult.Type.success);
            Assert.AreEqual(user, res.User, "User returned from LoginManager was not correct");

        }
    }
}
