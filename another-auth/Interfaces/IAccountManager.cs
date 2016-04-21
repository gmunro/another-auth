namespace another_auth.Interfaces
{
    public interface IAccountManager<T> where T : User
    {
        void CreateUserWithLogin(string userName, string password);
        LoginResult<T> ValidLogin(string userName, string password);
    }
}