using Microsoft.AspNetCore.Http;

namespace Postkit.Shared.Exceptions
{
    public class ValidationException : ApiException
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;

        public ValidationException(string message) : base(message)
        {
        }
    }
}
