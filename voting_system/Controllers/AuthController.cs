using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using voting_system.DTOS;
using voting_system.Models;

namespace voting_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            var user = _dbContext.Admins.FirstOrDefault(x => x.UserName.ToUpper() == loginDto.UserName.ToUpper());
            if (user == null)
            {
                return Unauthorized("Invalid Uresname Or Password");//401
            }
            //verify :that hashed password same as password entered by user without reversed hashed(return trur or false)
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHashed))
            {
                return Unauthorized("Invalid Uresname Or Password");
            }

            //token
            var token = GenerateJwtToken(user);
            return Ok(token);
        }
        private string GenerateJwtToken(Admin admin)
        {
            // Claims = information about the logged-in admin
            var claims = new List<Claim>();

            claims.Add(new Claim(
                ClaimTypes.NameIdentifier,
                admin.AdminId.ToString()
            ));

            claims.Add(new Claim(
                ClaimTypes.Name,
                admin.UserName
            ));

            // Role
            claims.Add(new Claim(
                ClaimTypes.Role,
                "Admin"
            ));

            // Secret key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("WHAFWEI#!@S!!112312WQEQW@RWQEQW324")
            );

            // Signing credentials
            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            // Token settings
            var tokenSetting = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.Now.AddDays(1)
            );

            // Create token
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(tokenSetting);

            return token;
        }
    }
}
