using System;

namespace another_auth
{
    public class StandardAccountManager : IAccountManager
    {
        private ILoginManager _loginManager;
        private IUserManager _userManager;

        public StandardAccountManager(IUserManager userManager, ILoginManager loginManager)
        {
            _userManager = userManager;
            _loginManager = loginManager;
        }

        public void CreateUserWithLogin(string userName, string password)
        {
            var user = _userManager.CreateUser(userName);
            _loginManager.CreateLogin(user, userName, password);
        }

        public LoginResult ValidLogin(string userName, string password)
        {
            return _loginManager.AttemptLogin(userName, password);
        }
    }
}