namespace another_auth.tests
{
    internal interface IAuthManager
    {
        void RegisterUser(string v);
        bool UserExistsByEmail(string v);
    }
}