namespace Postkit.Shared.Exceptions
{
    public class AlreadyExistsException : ConflictException
    {
        public AlreadyExistsException(string message = "Resource already exists.") : base(message) { }  
    }
}
