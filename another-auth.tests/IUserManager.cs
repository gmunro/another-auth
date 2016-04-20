namespace another_auth.tests
{
    internal interface IUserManager
    {
        User CreateUser(string primaryEmailAddress);
        bool UserExistsByEmail(string v);
    }
}