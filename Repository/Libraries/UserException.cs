namespace Repository.Libraries;

public class UserException : Exception
{
    public UserException(string message) : base(message) { }
}