namespace another_auth
{
    public class LoginResult
    {
        public enum Type
        {
            success,
            failiure
        };

        public Type ResultType { get; set; }
        public User User { get; set; }
    }
    public interface ILoginManager
    {
        void CreateLogin(User user, string loginUserName, string password);
        bool LoginExists(User user);
        LoginResult AttemptLogin(string primaryEmail, string password);
    }
}