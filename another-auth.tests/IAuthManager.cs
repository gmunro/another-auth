namespace another_auth.tests
{
    internal interface IAuthManager
    {
        UserAccount CreateUser(string primaryEmailAddress);
        bool UserExistsByEmail(string v);
        void CreateLogin(object user, object standard, string password);
        bool LoginExists(UserAccount user, AuthManager.LoginType standard);
    }
}