using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Restful.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/authentication")]
    public class AuthenticationController: Controller
    {
        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [HttpPost]
        public IActionResult Post()
        {
            var authorizationHeader = Request.Headers["Authorization"].First();
            var key = authorizationHeader.Split(' ')[1];
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(key)).Split(':');
            var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:ServerSecret"]));
            if (credentials[0] == "username" && credentials[1] == "password")
            {
                var result = new
                {
                    token = GenerateToken(serverSecret)
                };

                return Ok(result);
            }
            return BadRequest();
        }

        private string GenerateToken(SecurityKey key)
        {
            var now = DateTime.UtcNow;
            var issuer = Configuration["JWT:Issuer"];
            var audience = Configuration["JWT:Audience"];
            var identity = new ClaimsIdentity();
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(issuer, audience, identity,
            now, now.Add(TimeSpan.FromHours(1)), now, signingCredentials);
            var encodedJwt = handler.WriteToken(token);
            return encodedJwt;
        }
    }
}
