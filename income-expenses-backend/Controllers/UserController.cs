using IncomeExpenses.Data;
using IncomeExpenses.DTO;
using IncomeExpenses.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IncomeExpenses.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            var user = new Users
            {
                Username = request.Username.Trim(),
                FullName = request.FullName,
                CreateDate = DateTime.Now
            };

            var passwordHasher = new PasswordHasher<Users>();
            user.Password = passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Register successful"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var passwordHasher = new PasswordHasher<Users>();
            var verifyResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateJwtToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Username = user.Username,
                FullName = user.FullName
            });
        }


        private string GenerateJwtToken(Users user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
