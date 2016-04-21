using System;
using another_auth.Interfaces;

namespace another_auth
{
    public class StandardAccountManager<T> : IAccountManager<T> where T : User, new()
    {
        private ILoginManager<T> _loginManager;
        private IUserManager<T> _userManager;

        public StandardAccountManager(IUserManager<T> userManager, ILoginManager<T> loginManager)
        {
            _userManager = userManager;
            _loginManager = loginManager;
        }

        public StandardAccountManager(IAuthDb authDb, string applicationPepper)
        {
            var validator = new EmailAddressValidator();
            _userManager = new UserManager<T>(authDb, validator);
            _loginManager = new StandardLoginManager<T>(authDb, applicationPepper, validator);
        }

        public void CreateUserWithLogin(string userName, string password)
        {
            var user = _userManager.CreateUser(userName);
            _loginManager.CreateLogin(user, userName, password);
        }

        public LoginResult<T> ValidLogin(string userName, string password)
        {
            return _loginManager.AttemptLogin(userName, password);
        }
    }
}