using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace another_auth.tests
{
    [TestClass]
    public class RegisterAndLoginTests
    {
        private const string DefaultSitePepper = "RegisterAndLoginTestsPepper";
        private User CreateUserAccount(IAuthDb authDb, string primaryEmail)
        {
            IUserManager userManager = new UserManager(authDb);
            return userManager.CreateUser(primaryEmail);
        }

        private User CreateUserAccountWithStandardLogin(IAuthDb authDb, string primaryEmail, string password)
        {
            var user = CreateUserAccount(authDb, primaryEmail);
            ILoginManager loginManager = new StandardLoginManager(authDb, DefaultSitePepper);
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
            Assert.IsTrue(otherUserManager.UserExistsByEmail(primaryEmail), "New UserManager backed by same db, user did not exist.");
        }

        [TestMethod]
        public void LoginNonAuthenticatePersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = CreateUserAccountWithStandardLogin(authDb, primaryEmail, password);
            Assert.IsTrue(tAuthDb.SaveCalled);

            ILoginManager otherLoginManager = new StandardLoginManager(authDb, DefaultSitePepper);
            Assert.IsTrue(otherLoginManager.LoginExists(user), "LoginUsername did not persist through new LoginManager");
        }

        [TestMethod]
        public void LoginAuthenticatePersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = CreateUserAccountWithStandardLogin(authDb, primaryEmail, password);
            Assert.IsNotNull(user);
            Assert.IsTrue(tAuthDb.SaveCalled);

            var loginManager = new StandardLoginManager(authDb, DefaultSitePepper);
            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.AreEqual(LoginResult.Type.success, res.ResultType, "LoginManager returned failiure.");
            Assert.AreEqual(user, res.User, "User returned from LoginManager was not correct.");

        }

        [TestMethod]
        public void InvalidEmptySaltTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = CreateUserAccountWithStandardLogin(authDb, primaryEmail, password);
            Assert.IsNotNull(user);
            Assert.IsTrue(tAuthDb.SaveCalled);

            (tAuthDb.Backing[typeof(StandardLogin)][0] as StandardLogin).Salt = string.Empty;

            var loginManager = new StandardLoginManager(authDb, DefaultSitePepper);

            var threw = false;
            try
            {
                loginManager.AttemptLogin(primaryEmail, password);
            }
            catch (InvalidOperationException ex)
            {
                threw = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            Assert.IsTrue(threw);
        }

        [TestMethod]
        public void InvalidIncorrectSaltTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = CreateUserAccountWithStandardLogin(authDb, primaryEmail, password);
            Assert.IsNotNull(user);
            Assert.IsTrue(tAuthDb.SaveCalled);
            var login = tAuthDb.Backing[typeof(StandardLogin)][0] as StandardLogin;
            login.Salt = $"{login.Salt}1";

            var loginManager = new StandardLoginManager(authDb, DefaultSitePepper);

            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.AreEqual(LoginResult.Type.failiure, res.ResultType,"LoginManager allowed user to login despite salt having changed");

        }

        [TestMethod]
        public void InvalidPasswordLoginTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = CreateUserAccountWithStandardLogin(authDb, primaryEmail, password);
            Assert.IsNotNull(user);
            Assert.IsTrue(tAuthDb.SaveCalled);

            var loginManager = new StandardLoginManager(authDb, DefaultSitePepper);
            var res = loginManager.AttemptLogin(primaryEmail, $"{password}z");

            Assert.AreEqual(LoginResult.Type.failiure, res.ResultType, "LoginManager incorrectly authenticated login.");
        }

        [TestMethod]
        public void InvalidSitePepperLoginTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            const string password = "zzz1";

            var user = CreateUserAccountWithStandardLogin(authDb, primaryEmail, password);
            Assert.IsNotNull(user);
            Assert.IsTrue(tAuthDb.SaveCalled);

            var loginManager = new StandardLoginManager(authDb, $"{DefaultSitePepper}1");
            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.AreEqual(LoginResult.Type.failiure, res.ResultType, "LoginManager incorrectly authenticated login.");
        }
    }
}
