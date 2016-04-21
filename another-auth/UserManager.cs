using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using another_auth.Exceptions;
using another_auth.Interfaces;

namespace another_auth
{
    public class UserManager<T> : IUserManager<T> where T : User, new()
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

        public T CreateUser(string primaryEmailAddress)
        {
            if (!_emailAddressValidator.IsValid(primaryEmailAddress))
            {
                throw new InvalidDataException("Unable to CreateUser, Email address was not in expected format.");
            }

            if (_authDb.ModelPresent<T>() && _authDb.Query<T>().Any(p => p.PrimaryEmailAddress.Equals(primaryEmailAddress)))
            {
                throw new DuplicateAccountException();
            }

            var user = new T
            {
                PrimaryEmailAddress = primaryEmailAddress
            };
            _authDb.Add<T>(user);
            _authDb.Save();
            return user;
        }

        public bool UserExistsByEmail(string v)
        {
            var accounts = _authDb.Query<T>();
            return accounts.Any(p => string.Equals(v, p.PrimaryEmailAddress));
        }
    }
}