using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using another_auth.Interfaces;

namespace another_auth.sample
{
    class Program
    {
        static void Main(string[] args)
        {
            IAuthDb authDb = new AuthDb();
            var pepper = "changme";

            var accountManager = new StandardAccountManager<SampleUser, SampleLogin>(authDb, pepper);

            accountManager.CreateUserWithLogin("foo@bar.com", "password1");
           
            var result = accountManager.ValidLogin("foo@bar.com", "password1");

            if (result.ResultType == LoginResult<SampleUser>.Type.success)
            {
                Console.WriteLine($"User {result.User.PrimaryEmailAddress} logged in OK");
            }
            else if (result.ResultType == LoginResult<SampleUser>.Type.failiure)
            {
                Console.WriteLine($"Unable to login");
            }
            else
            {
                throw new Exception("An unexpected value was returned from the AccountManger");
            }

            Console.ReadLine();
        }
    }
}
