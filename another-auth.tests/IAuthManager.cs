namespace another_auth.tests
{
    internal interface IAuthManager
    {
        void RegisterUser(string primaryEmailAddress);
        bool UserExistsByEmail(string v);
    }
}