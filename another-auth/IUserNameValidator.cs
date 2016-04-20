namespace another_auth
{
    public interface IUserNameValidator
    {
        bool IsValid(string primaryEmail);
    }
}