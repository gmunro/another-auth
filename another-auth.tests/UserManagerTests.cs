using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using another_auth.Exceptions;
using another_auth.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace another_auth.tests
{
    [TestClass]
    public class UserManagerTests
    {
        public static TestUser CreateUserAccount(IAuthDb authDb, string primaryEmail)
        {
            IUserManager<TestUser> userManager = new UserManager<TestUser>(authDb, new EmailAddressValidator());
            return userManager.CreateUser(primaryEmail);
        }



        [TestMethod]
        public void InvalidPrimaryEmailAddressTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu @gmail.com";

            Assert.IsFalse(new EmailAddressValidator().IsValid(primaryEmail), "EmailAddressValidator accepted invalid username");

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
        public void UserPersists()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            CreateUserAccount(authDb, primaryEmail);
            Assert.IsTrue(tAuthDb.SaveCalled, "Save was not called on db");

            IUserManager<TestUser> otherUserManager = new UserManager<TestUser>(authDb, new EmailAddressValidator());
            Assert.IsTrue(otherUserManager.UserExistsByEmail(primaryEmail), "New UserManager backed by same db, user did not exist.");
        }
        [TestMethod]
        public void NoDuplicatePrimaryEmailAddressTest()
        {
            var tAuthDb = new TestAuthDb();
            IAuthDb authDb = tAuthDb;

            const string primaryEmail = "garethmu@gmail.com";
            CreateUserAccount(authDb, primaryEmail);

            var threw = false;
            try
            {

                CreateUserAccount(authDb, primaryEmail);
            }
            catch (DuplicateAccountException)
            {
                threw = true;
            }

            Assert.IsTrue(threw);
        }


    }
}
