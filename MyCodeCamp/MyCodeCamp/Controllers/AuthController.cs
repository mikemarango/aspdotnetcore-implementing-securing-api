using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IPasswordHasher<CampUser> passwordHasher;
        private readonly SignInManager<CampUser> signInManager;
        private readonly TokenSettings tokenSettings;
        private readonly UserManager<CampUser> userManager;

        public AuthController(CampContext context, ILogger<AuthController> logger, IOptions<TokenSettings> optionsAccessor, IPasswordHasher<CampUser> passwordHasher, SignInManager<CampUser> signInManager, UserManager<CampUser> userManager)
        {
            this.context = context;
            this.logger = logger;
            this.passwordHasher = passwordHasher;
            this.signInManager = signInManager;
            tokenSettings = optionsAccessor.Value;
            this.userManager = userManager;
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

        [HttpPost("api/auth/token")]
        [ValidateModel]
        public async Task<IActionResult> CreateToken([FromBody] CredentialModel model)
        {
            try
            {
                var user = await userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                    {
                        var userClaims = await userManager.GetClaimsAsync(user);

                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        }.Union(userClaims);

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key));

                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: tokenSettings.Issuer,
                            audience: tokenSettings.Audience,
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(15),
                            signingCredentials: creds
                            );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError("Threw exception while creating JWT", exception);
            }

            return BadRequest("Failed to generate token");
        }
    }
}
