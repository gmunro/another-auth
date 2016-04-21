using System;
using another_auth.Interfaces;

namespace another_auth
{
    public class StandardAccountManager<TUser, TLogin> : IAccountManager<TUser> where TUser : User, new() where TLogin : Login<TUser>, new()
    {
        private ILoginManager<TUser> _loginManager;
        private IUserManager<TUser> _userManager;

        public StandardAccountManager(IUserManager<TUser> userManager, ILoginManager<TUser> loginManager)
        {
            _userManager = userManager;
            _loginManager = loginManager;
        }

        public StandardAccountManager(IAuthDb authDb, string applicationPepper)
        {
            var validator = new EmailAddressValidator();
            _userManager = new UserManager<TUser>(authDb, validator);
            _loginManager = new LoginManager<TUser,TLogin>(authDb, applicationPepper, validator);
        }

        public void CreateUserWithLogin(string userName, string password)
        {
            var user = _userManager.CreateUser(userName);
            _loginManager.CreateLogin(user, userName, password);
        }

        public LoginResult<TUser> ValidLogin(string userName, string password)
        {
            return _loginManager.AttemptLogin(userName, password);
        }
    }
}