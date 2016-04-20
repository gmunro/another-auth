namespace another_auth
{
    public interface IUserManager
    {
        User CreateUser(string primaryEmailAddress);
        bool UserExistsByEmail(string v);
    }
}