using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace another_auth.tests
{
    internal class UserManager : IUserManager
    {
        private IAuthDb authDb;

        public enum LoginType
        {
            Standard
        };

        public UserManager(IAuthDb authDb)
        {
            this.authDb = authDb;
        }

        public User CreateUser(string primaryEmailAddress)
        {
            var user = new User
            {
                PrimaryEmailAddress = primaryEmailAddress
            };
            authDb.Add<User>(user);
            authDb.Save();
            return user;
        }

        public bool UserExistsByEmail(string v)
        {
            var accounts = authDb.Query<User>();
            return accounts.Any(p => string.Equals(v, p.PrimaryEmailAddress));
        }
    }
}