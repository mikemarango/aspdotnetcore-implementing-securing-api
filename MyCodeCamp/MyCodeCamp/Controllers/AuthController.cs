using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;

namespace MyCodeCamp.Controllers
{
    public class AuthController : Controller
    {
        private CampContext context;
        private readonly ILogger<AuthController> logger;
        private readonly SignInManager<CampUser> signInManager;

        public AuthController(CampContext context, ILogger<AuthController> logger, SignInManager<CampUser> signInManager)
        {
            this.context = context;
            this.logger = logger;
            this.signInManager = signInManager;
        }

        [HttpPost("api/auth/login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                if (result.Succeeded)
                    return Ok();
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while logging in", exception);
            }

            return BadRequest("Failed to login");
        }
    }
}
