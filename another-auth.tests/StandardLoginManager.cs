using System;
using System.Linq;

namespace another_auth.tests
{
    internal class StandardLoginManager : ILoginManager
    {
        private IAuthDb authDb;

        public StandardLoginManager(IAuthDb authDb)
        {
            this.authDb = authDb;
        }

        public void CreateLogin(User user, string password)
        {
            var login = new StandardLogin
            {
                Login = user.PrimaryEmailAddress,
                Password = password,
                User = user
            };
            authDb.Add<StandardLogin>(login);
            authDb.Save();
        }

        public bool LoginExists(User user)
        {
            return authDb.Query<StandardLogin>().Any(p => p.User == user);
        }
    }
}