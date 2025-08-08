using Microsoft.AspNetCore.Http;

namespace Postkit.Shared.Exceptions
{
    public class NotFoundException : ApiException
    {
        public override int StatusCode => StatusCodes.Status404NotFound;
        public NotFoundException(string message = "Resource not found"): base(message) {}
    }
}
