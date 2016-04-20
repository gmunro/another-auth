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

        public void CreateLogin(User user, string loginUsername, string password)
        {
            // Generate a random salt for this user
            var salt = GetRandomSalt();
            var login = new StandardLogin
            {
                LoginUsername = loginUsername,
                Salt = salt,
                Hash = GetHash(salt,password),
                User = user
            };
            authDb.Add<StandardLogin>(login);
            authDb.Save();
        }

        public bool LoginExists(User user)
        {
            return authDb.Query<StandardLogin>().Any(p => p.User == user);
        }

        internal LoginResult AttemptLogin(string loginUsername, string password)
        {
            var login = authDb.Query<StandardLogin>().FirstOrDefault(p => string.Equals(p.LoginUsername, loginUsername));

            if (login == null)
            {
                return new LoginResult
                {
                    ResultType = LoginResult.Type.failiure
                };
            }

            var providedHash = GetHash(login.Salt, password);
            if (string.IsNullOrWhiteSpace(providedHash) || string.IsNullOrWhiteSpace(login.Salt))
            {
                // In the event that a stored hash is invalid, or the calculated one is invalid
                // do not allow the user to log in.
                return new LoginResult
                {
                    ResultType = LoginResult.Type.failiure
                };
                // Todo log this error case
            }

            if (!string.Equals(login.Hash, providedHash))
            {
                return new LoginResult
                {
                    ResultType = LoginResult.Type.failiure,
                    // User may be desired to implement attempt logging, notifications etc.
                    //User = login.User
                };
            }

            return new LoginResult
            {
                ResultType = LoginResult.Type.success,
                User = login.User
            };
        }

        private string GetRandomSalt()
        {
            return null;
        }

        private string GetHash(string salt, string password)
        {
            return null;
        }
    }
}