using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            ILoginManager loginManager = new StandardLoginManager(authDb, DefaultSitePepper, _userNameValidator);
            IUserManager userManager = new UserManager(authDb, new EmailAddressValidator());

            IAccountManager accountManager = new StandardAccountManager(userManager, loginManager);

            string userName = "garethmu@gmail.com";
            string password = "zzz1";

            accountManager.CreateUserWithLogin(userName, password);

            Assert.AreEqual(LoginResult.Type.success, accountManager.ValidLogin(userName, password).ResultType, "Newly created user account failed to login");
        }

        [TestMethod]
        public void AccountManagerCreatesUserPersistsTest()
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

            Assert.AreEqual(LoginResult.Type.success, accountManager2.ValidLogin(userName, password).ResultType, "created user account did not persist");
        }
    }
}
