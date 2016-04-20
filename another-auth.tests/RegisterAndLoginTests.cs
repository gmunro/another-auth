using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace another_auth.tests
{
    [TestClass]
    public class RegisterAndLoginTests
    {
        private const string DefaultSitePepper = "RegisterAndLoginTestsPepper";
        private readonly IUserNameValidator _userNameValidator = new EmailAddressValidator();
        private User CreateUserAccount(IAuthDb authDb, string primaryEmail)
        {
            IUserManager userManager = new UserManager(authDb, new EmailAddressValidator());
            return userManager.CreateUser(primaryEmail);
        }

        private User CreateUserAccountWithStandardLogin(IAuthDb authDb, string primaryEmail, string password)
        {
            var user = CreateUserAccount(authDb, primaryEmail);
            ILoginManager loginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);
            loginManager.CreateLogin(user, user.PrimaryEmailAddress, password);
            return user;
        }

        [TestMethod]
        public void InvalidPrimaryEmailAddressTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu @gmail.com";

            Assert.IsFalse(_userNameValidator.IsValid(primaryEmail),"EmailAddressValidator accepted invalid username");

            var threw = false;
            try
            {
                CreateUserAccount(authDb, primaryEmail);
            }
            catch (InvalidDataException)
            {
                threw = true;
            }
            Assert.IsTrue(threw, "Creating user account did not throw expected error with invalid email address");
        }

        [TestMethod]
        public void InvalidLoginUserNameAddressTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu @gmail.com";

            var lm = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);

            var threw = false;
            try
            {

                lm.CreateLogin(null, primaryEmail, "password");
            }
            catch (InvalidDataException)
            {
                threw = true;
            }
            Assert.IsTrue(threw,"LoginManager did not throw error with invalid login name");
        }

        [TestMethod]
        public void UserPersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            CreateUserAccount(authDb, primaryEmail);
            Assert.IsTrue(tAuthDb.SaveCalled, "Save was not called on db");

            IUserManager otherUserManager = new UserManager(authDb, new EmailAddressValidator());
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

            ILoginManager otherLoginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);
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

            ILoginManager loginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);
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

            var loginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);

            var threw = false;
            try
            {
                loginManager.AttemptLogin(primaryEmail, password);
            }
            catch (InvalidOperationException)
            {
                threw = true;
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

            ILoginManager loginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);

            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.AreEqual(LoginResult.Type.failiure, res.ResultType, "LoginManager allowed user to login despite salt having changed");

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

            ILoginManager loginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);
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

            ILoginManager loginManager = new StandardLoginManager(authDb, $"{DefaultSitePepper}1", _userNameValidator);
            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.AreEqual(LoginResult.Type.failiure, res.ResultType, "LoginManager incorrectly authenticated login.");
        }

        [TestMethod]
        public void AssureDistinctSaltTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string password = "zzz1";

            var user1 = CreateUserAccountWithStandardLogin(authDb, "garethmu@gmail.com", password);
            var user2 = CreateUserAccountWithStandardLogin(authDb, "foo@bar.com", password);

            var login1 = tAuthDb.Backing[typeof(StandardLogin)][0] as StandardLogin;
            var login2 = tAuthDb.Backing[typeof(StandardLogin)][1] as StandardLogin;

            Assert.AreNotEqual(login1.Salt, login2.Salt, "Two user accounts shared the same salt");
        }

        [TestMethod]
        public void AssureDistinctHashTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string password = "zzz1";

            var user1 = CreateUserAccountWithStandardLogin(authDb, "garethmu@gmail.com", password);
            var user2 = CreateUserAccountWithStandardLogin(authDb, "foo@bar.com", password);

            var login1 = tAuthDb.Backing[typeof(StandardLogin)][0] as StandardLogin;
            var login2 = tAuthDb.Backing[typeof(StandardLogin)][1] as StandardLogin;

            Assert.AreNotEqual(login1.Hash, login2.Hash, "Two user accounts shared the same hash");
        }

        [TestMethod]
        public void LoginManagerCreatesUserTest()
        {
            var authDb = new TestAuthDb();

            ILoginManager loginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);
            IUserManager userManager = new UserManager(authDb, new EmailAddressValidator());

            IAccountManager accountManager= new StandardAccountManager(userManager, loginManager);

            string userName = "garethmu@gmail.com";
            string password = "zzz1";

            accountManager.CreateUserWithLogin(userName, password);

            Assert.AreEqual(LoginResult.Type.success,accountManager.ValidLogin(userName, password).ResultType,"Newly created user account failed to login");
        }

        [TestMethod]
        public void LoginManagerCreatesUserPersistsTest()
        {
            var authDb = new TestAuthDb();

            ILoginManager loginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);
            IUserManager userManager = new UserManager(authDb, new EmailAddressValidator());

            IAccountManager accountManager = new StandardAccountManager(userManager, loginManager);

            string userName = "garethmu@gmail.com";
            string password = "zzz1";

            accountManager.CreateUserWithLogin(userName, password);

            ILoginManager loginManager2 = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);
            IUserManager userManager2 = new UserManager(authDb, new EmailAddressValidator());

            IAccountManager accountManager2 = new StandardAccountManager(userManager2, loginManager2);

            Assert.AreEqual(LoginResult.Type.success, accountManager2.ValidLogin(userName, password).ResultType,"created user account did not persist");
        }
    }
}
