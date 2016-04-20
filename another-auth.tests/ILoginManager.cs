namespace another_auth.tests
{
    class LoginResult
    {
        public enum Type
        {
            success,
            failiure
        };

        public Type ResultType { get; set; }
        public User User { get; set; }
    }
    internal interface ILoginManager
    {
        void CreateLogin(User user, string loginUserName, string password);
        bool LoginExists(User user);
    }
}