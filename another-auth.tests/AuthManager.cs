using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace another_auth.tests
{
    internal class AuthManager : IAuthManager
    {
        private IAuthDb authDb;

        public enum LoginType
        {
            Standard
        };

        public AuthManager(IAuthDb authDb)
        {
            this.authDb = authDb;
        }

        public UserAccount CreateUser(string primaryEmailAddress)
        {
            var user = new UserAccount
            {
                PrimaryEmailAddress = primaryEmailAddress
            };
            authDb.Add<UserAccount>(user);
            authDb.Save();
            return user;
        }

        public bool UserExistsByEmail(string v)
        {
            var accounts = authDb.Query<UserAccount>();
            return accounts.Any(p => string.Equals(v, p.PrimaryEmailAddress));
        }

        public void CreateLogin(object user, object standard, string password)
        {
            throw new NotImplementedException();
        }

        public bool LoginExists(UserAccount user, LoginType standard)
        {
            throw new NotImplementedException();
        }
    }
}