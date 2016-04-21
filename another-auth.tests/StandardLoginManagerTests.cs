using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using another_auth.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace another_auth.tests
{
    [TestClass]
    public class StandardLoginManagerTests
    {
        private const string DefaultSitePepper = "RegisterAndLoginTestsPepper";
        private static readonly IUserNameValidator _userNameValidator = new EmailAddressValidator();
        private static TestUser CreateUserAccountWithStandardLogin(IAuthDb authDb, string primaryEmail, string password)
        {
            var user = UserManagerTests.CreateUserAccount(authDb, primaryEmail);
            ILoginManager<TestUser> loginManager = new StandardLoginManager<TestUser>(authDb, DefaultSitePepper, _userNameValidator);
            loginManager.CreateLogin(user, user.PrimaryEmailAddress, password);
            return user;
        }
        [TestMethod]
        public void InvalidLoginUserNameAddressTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu @gmail.com";

            var lm = new StandardLoginManager<TestUser>(authDb, DefaultSitePepper, _userNameValidator);

            var threw = false;
            try
            {

                lm.CreateLogin(null, primaryEmail, "password");
            }
            catch (InvalidDataException)
            {
                threw = true;
            }
            Assert.IsTrue(threw, "LoginManager did not throw error with invalid login name");
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

            ILoginManager<TestUser> loginManager = new StandardLoginManager<TestUser>(authDb, DefaultSitePepper, _userNameValidator);
            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.AreEqual(LoginResult<TestUser>.Type.success, res.ResultType, "LoginManager returned failiure.");
            Assert.AreEqual(user, res.User, "User returned from LoginManager was not correct.");

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

            ILoginManager<TestUser> otherLoginManager = new StandardLoginManager<TestUser>(authDb, DefaultSitePepper, _userNameValidator);
            Assert.IsTrue(otherLoginManager.LoginExists(user), "LoginUsername did not persist through new LoginManager");
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

            (tAuthDb.Backing[typeof(StandardLogin<TestUser>)][0] as StandardLogin<TestUser>).Salt = string.Empty;

            var loginManager = new StandardLoginManager<TestUser>(authDb, DefaultSitePepper, _userNameValidator);

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
            var login = tAuthDb.Backing[typeof(StandardLogin<TestUser>)][0] as StandardLogin<TestUser>;
            login.Salt = $"{login.Salt}1";

            ILoginManager<TestUser> loginManager = new StandardLoginManager<TestUser>(authDb, DefaultSitePepper, _userNameValidator);

            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.AreEqual(LoginResult<TestUser>.Type.failiure, res.ResultType, "LoginManager allowed user to login despite salt having changed");

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

            ILoginManager<TestUser> loginManager = new StandardLoginManager<TestUser>(authDb, DefaultSitePepper, _userNameValidator);
            var res = loginManager.AttemptLogin(primaryEmail, $"{password}z");

            Assert.AreEqual(LoginResult<TestUser>.Type.failiure, res.ResultType, "LoginManager incorrectly authenticated login.");
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

            ILoginManager<TestUser> loginManager = new StandardLoginManager<TestUser>(authDb, $"{DefaultSitePepper}1", _userNameValidator);
            var res = loginManager.AttemptLogin(primaryEmail, password);

            Assert.AreEqual(LoginResult<TestUser>.Type.failiure, res.ResultType, "LoginManager incorrectly authenticated login.");
        }

        [TestMethod]
        public void AssureDistinctSaltTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string password = "zzz1";

            var user1 = CreateUserAccountWithStandardLogin(authDb, "garethmu@gmail.com", password);
            var user2 = CreateUserAccountWithStandardLogin(authDb, "foo@bar.com", password);

            var login1 = tAuthDb.Backing[typeof(StandardLogin<TestUser>)][0] as StandardLogin<TestUser>;
            var login2 = tAuthDb.Backing[typeof(StandardLogin<TestUser>)][1] as StandardLogin<TestUser>;

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

            var login1 = tAuthDb.Backing[typeof(StandardLogin<TestUser>)][0] as StandardLogin<TestUser>;
            var login2 = tAuthDb.Backing[typeof(StandardLogin<TestUser>)][1] as StandardLogin<TestUser>;

            Assert.AreNotEqual(login1.Hash, login2.Hash, "Two user accounts shared the same hash");
        }


    }
}
