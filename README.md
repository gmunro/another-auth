# another-auth

### To use in default configuration
- Implement IAuthDb with your backing of choice (e.g Entity Framework)
- choose a *pepper* unique to an installation of the application using the library. Note the pepper is not likely to pracitcally make any difference to security of the stored password hashes, though it may in some obscure circumstances.
```
var accountManager = new StandardAccountManager(authDb,applicationPepper)

accountManager.CreateUserWithLogin("foo@bar.com", "password1");

var result = accountManager.ValidLogin("foo@bar.com", "password1");

            if (result.ResultType == LoginResult.Type.success)
            {
                Console.WriteLine($"User {result.User.PrimaryEmailAddress} logged in OK");
            }
```

### See another-auth.sample for compilable sample.
