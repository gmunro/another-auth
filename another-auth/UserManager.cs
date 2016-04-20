using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace another_auth
{
    public class UserManager : IUserManager
    {
        private readonly IAuthDb _authDb;
        private readonly EmailAddressValidator _emailAddressValidator;

        public enum LoginType
        {
            Standard
        };

        public UserManager(IAuthDb authDb, EmailAddressValidator emailValidator)
        {
            _authDb = authDb;
            _emailAddressValidator = emailValidator;
        }

        public User CreateUser(string primaryEmailAddress)
        {
            if (!_emailAddressValidator.IsValid(primaryEmailAddress))
            {
                throw new InvalidDataException("Unable to CreateUser, Email address was not in expected format.");
            }
            var user = new User
            {
                PrimaryEmailAddress = primaryEmailAddress
            };
            _authDb.Add<User>(user);
            _authDb.Save();
            return user;
        }

        public bool UserExistsByEmail(string v)
        {
            var accounts = _authDb.Query<User>();
            return accounts.Any(p => string.Equals(v, p.PrimaryEmailAddress));
        }
    }
}