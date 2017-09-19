using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Primitives;

namespace MyCodeCamp.Controllers
{

    [Route("api/[controller]")]
    public class OperationsController : Controller
    {
        private readonly ILogger<SpeakersController> logger;
        private readonly IConfiguration configuration;

        public OperationsController(ILogger<SpeakersController> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        [HttpGet("reloadConfig")]
        public IActionResult ReloadConfiguration()
        {
            try
            {
                var configurationRoot = configuration as IConfigurationRoot;

                configurationRoot?.Reload();

                return Ok("Configuration reloaded");
            }
            catch (Exception exception)
            {
                logger.LogError("Exception thrown while reloading configuration", exception);
            }

            return BadRequest("Could not reload configuration");
        }
    }
}
