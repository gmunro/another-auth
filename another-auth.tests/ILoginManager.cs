namespace another_auth.tests
{
    internal interface ILoginManager
    {
        void CreateLogin(User user, string password);
        bool LoginExists(User user);
    }
}