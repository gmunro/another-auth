using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using another_auth.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace another_auth.tests
{
    [TestClass]
    public class StandardAccountManagerTests
    {
        private const string DefaultSitePepper = "RegisterAndLoginTestsPepper";
        private readonly IUserNameValidator _userNameValidator = new EmailAddressValidator();
        [TestMethod]
        public void AccountManagerCreatesUserTest()
        {
            var authDb = new TestAuthDb();

            ILoginManager<TestUser> loginManager = new LoginManager<TestUser,TestLogin>(authDb, DefaultSitePepper, _userNameValidator);
            IUserManager<TestUser> userManager = new UserManager<TestUser>(authDb, new EmailAddressValidator());

            IAccountManager<TestUser> accountManager = new StandardAccountManager<TestUser, TestLogin>(userManager, loginManager);

            string userName = "garethmu@gmail.com";
            string password = "zzz1";

            accountManager.CreateUserWithLogin(userName, password);

            Assert.AreEqual(LoginResult<TestUser>.Type.success, accountManager.ValidLogin(userName, password).ResultType, "Newly created user account failed to login");
        }

        [TestMethod]
        public void AccountManagerCreatesUserPersistsTest()
        {
            var authDb = new TestAuthDb();

            ILoginManager<TestUser> loginManager = new LoginManager<TestUser, TestLogin>(authDb, DefaultSitePepper, _userNameValidator);
            IUserManager<TestUser> userManager = new UserManager<TestUser>(authDb, new EmailAddressValidator());

            IAccountManager<TestUser> accountManager = new StandardAccountManager<TestUser, TestLogin>(userManager, loginManager);

            string userName = "garethmu@gmail.com";
            string password = "zzz1";

            accountManager.CreateUserWithLogin(userName, password);

            ILoginManager<TestUser> loginManager2 = new LoginManager<TestUser, TestLogin>(authDb, DefaultSitePepper, _userNameValidator);
            IUserManager<TestUser> userManager2 = new UserManager<TestUser>(authDb, new EmailAddressValidator());

            IAccountManager<TestUser> accountManager2 = new StandardAccountManager<TestUser, TestLogin>(userManager2, loginManager2);

            Assert.AreEqual(LoginResult<TestUser>.Type.success, accountManager2.ValidLogin(userName, password).ResultType, "created user account did not persist");
        }
    }
}
