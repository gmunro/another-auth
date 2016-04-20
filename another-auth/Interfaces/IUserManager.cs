namespace another_auth.Interfaces
{
    public interface IUserManager
    {
        User CreateUser(string primaryEmailAddress);
        bool UserExistsByEmail(string v);
    }
}