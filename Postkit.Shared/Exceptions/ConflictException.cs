using Microsoft.AspNetCore.Http;

namespace Postkit.Shared.Exceptions
{
    public class ConflictException : ApiException
    {
        public override int StatusCode => StatusCodes.Status409Conflict;
        public ConflictException(string message = "Conflict occurred during the request.") : base(message) { }
    }
}
