namespace another_auth
{
    public interface IAccountManager
    {
        void CreateUserWithLogin(string userName, string password);
        LoginResult ValidLogin(string userName, string password);
    }
}