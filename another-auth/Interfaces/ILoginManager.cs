namespace another_auth.Interfaces
{
    public class LoginResult<T> where T : User
    {
        // Todo refactor this out
        public enum Type
        {
            success,
            failiure
        };

        public Type ResultType { get; set; }
        public T User { get; set; }
    }
    public interface ILoginManager<T> where T : User
    {
        void CreateLogin(T user, string loginUserName, string password);
        bool LoginExists(T user);
        LoginResult<T> AttemptLogin(string primaryEmail, string password);
    }
}