namespace another_auth.tests
{
    internal interface IUserNameValidator
    {
        bool IsValid(string primaryEmail);
    }
}