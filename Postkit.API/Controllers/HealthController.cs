using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/health")]
    [SwaggerTag("Provides basic health status of the API.")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> logger;

        public HealthController(ILogger<HealthController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Returns the health status of the API.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            logger.LogInformation("Health check pinged at {Time}", DateTime.UtcNow);

            return Ok(new
            {
                status = "Healthy",
                app = "Postkit.API",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
