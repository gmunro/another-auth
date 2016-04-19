namespace another_auth.tests
{
    internal class AuthManager : IAuthManager
    {
        private IAuthDb authDb;

        public AuthManager(IAuthDb authDb)
        {
            this.authDb = authDb;
        }

        public void RegisterUser(string v)
        {
            
        }

        public bool UserExistsByEmail(string v)
        {
            return false;
        }

    }
}