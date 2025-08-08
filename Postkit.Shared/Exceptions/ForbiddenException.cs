using Microsoft.AspNetCore.Http;

namespace Postkit.Shared.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public override int StatusCode => StatusCodes.Status403Forbidden;
        public ForbiddenException(string message = "Forbidden access") : base(message) { }
    }
}
