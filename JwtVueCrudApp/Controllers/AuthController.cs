using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using JwtVueCrudApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace JwtVueCrudApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (_dbContext.Users.Any(u => u.UserName == model.UserName))
                {
                    return BadRequest("Username already exists");
                }

                var user = new User { UserName = model.UserName, Password = BCrypt.Net.BCrypt.HashPassword(model.Password) };
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try { 
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.SingleOrDefault(u => u.UserName == model.UserName);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    string accessToken = GenerateAccessToken(user);
                    string refreshToken = GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                    await _dbContext.SaveChangesAsync();
                    return Ok(new { Token = accessToken, RefreshToken = refreshToken });
                }
                else
                {
                    return Unauthorized();
                }
            }
            return BadRequest(ModelState);
                            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto model)
        {
            if (ModelState.IsValid)
            {
                // User에 refresh token이 있는지 확인. 그리고 refresh token이 만료되지 않았는지 확인.
                var user = _dbContext.Users.SingleOrDefault(u => u.RefreshToken == model.RefreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);

                if (user != null)
                {
                    string accessToken = GenerateAccessToken(user);
                    return Ok(new { Token = accessToken });
                }
                else
                {
                    return Unauthorized();
                }
            }
            return BadRequest(ModelState);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
