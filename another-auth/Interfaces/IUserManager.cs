namespace another_auth.Interfaces
{
    public interface IUserManager<T> where T : User, new()
    {
        T CreateUser(string primaryEmailAddress);
        bool UserExistsByEmail(string v);
    }
}