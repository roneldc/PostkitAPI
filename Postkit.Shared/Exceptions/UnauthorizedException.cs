using Microsoft.AspNetCore.Http;

namespace Postkit.Shared.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public override int StatusCode => StatusCodes.Status401Unauthorized;
        public UnauthorizedException(string message = "Unauthorized access") : base(message) { }
    }
}
