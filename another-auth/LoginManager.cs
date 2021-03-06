﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using another_auth.Interfaces;
using Scrypt;

namespace another_auth
{
    public class LoginManager<TUser,TLogin> : ILoginManager<TUser> where TUser : User where TLogin : Login<TUser>, new()
    {
        private readonly RNGCryptoServiceProvider _rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        private readonly ScryptEncoder _encoder = new ScryptEncoder();
        private readonly IAuthDb _authDb;
        private readonly string _sitePepper;
        private readonly IUserNameValidator _userNameValidator;

        public LoginManager(IAuthDb authDb, string sitePepper, IUserNameValidator userNameValidator)
        {
            _authDb = authDb;
            _sitePepper = sitePepper;
            _userNameValidator = userNameValidator;
        }

        public void CreateLogin(TUser user, string loginUsername, string password)
        {
            if (!_userNameValidator.IsValid(loginUsername))
            {
                throw new InvalidDataException("LoginUsername was not provided in the expected format");
            }
            // Generate a random salt for this user
            var salt = GetRandomSalt();
            var login = new TLogin
            {
                LoginUsername = loginUsername,
                Salt = salt,
                Hash = GetHash(salt, password),
                User = user
            };
            _authDb.Add<TLogin>(login);
            _authDb.Save();
        }

        public bool LoginExists(TUser user)
        {
            return _authDb.Query<TLogin>().Any(p => p.User == user);
        }
        public LoginResult<TUser> AttemptLogin(string loginUsername, string password)
        {
            var login = _authDb.Query<TLogin>().FirstOrDefault(p => string.Equals(p.LoginUsername, loginUsername));

            // If there was no login found for the loginUsername, or the user was not found on it
            if (login?.User == null)
            {
                return new LoginResult<TUser>
                {
                    ResultType = LoginResult<TUser>.Type.failiure
                };
            }

            if (string.IsNullOrWhiteSpace(_sitePepper) || string.IsNullOrWhiteSpace(login.Salt))
            {
                throw new InvalidOperationException("An error occured during secure login");
            }

            // If the hashes do not match
            if (!_encoder.Compare(GetSaltedAndPepperedPassword(login.Salt, password), login.Hash))
            {
                return new LoginResult<TUser>
                {
                    ResultType = LoginResult<TUser>.Type.failiure,
                    // User may be desired to implement attempt logging, notifications etc.
                    //User = login.User
                };
            }

            // At this stage there is a valid login attempt
            return new LoginResult<TUser>
            {
                ResultType = LoginResult<TUser>.Type.success,
                User = login.User
            };
        }

        private string GetRandomSalt()
        {
            // todo perhaps salt length should be configurable
            var bytes = new byte[64];
            _rngCryptoServiceProvider.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private string GetHash(string salt, string password)
        {
            var passwordToHash = GetSaltedAndPepperedPassword(salt, password);
            string hashsedPassword = _encoder.Encode(passwordToHash);

            return hashsedPassword;
        }

        private string GetSaltedAndPepperedPassword(string salt, string password)
        {
            return $"{salt}{password}{_sitePepper}";
        }
    }
}