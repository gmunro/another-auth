namespace another_auth.tests
{
    internal interface IAccountManager
    {
        void CreateUserWithLogin(string userName, string password);
        LoginResult ValidLogin(string userName, string password);
    }
}