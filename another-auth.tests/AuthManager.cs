using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace another_auth.tests
{
    internal class AuthManager : IAuthManager
    {
        private IAuthDb authDb;

        public AuthManager(IAuthDb authDb)
        {
            this.authDb = authDb;
        }

        public void RegisterUser(string primaryEmailAddress)
        {
            authDb.Add<UserAccount>(new UserAccount
            {
                PrimaryEmailAddress = primaryEmailAddress
            });
            authDb.Save();
        }

        public bool UserExistsByEmail(string v)
        {
            var accounts = authDb.Query<UserAccount>();
            return accounts.Any(p => string.Equals(v, p.PrimaryEmailAddress));
        }

    }
}