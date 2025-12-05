using LinenManagementAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace linen_management_system.Helper
{
    public interface IHelper
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hash);
        public string GenerateAccessToken(Employee employee);

        public string GenerateRefreshToken();

    }
    public class Helper : IHelper
    {
        private readonly IConfiguration _config;  
        
        public Helper(IConfiguration config)
        {
            _config = config;   
        }
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public string GenerateAccessToken(Employee employee)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
        new Claim(ClaimTypes.Email, employee.Email),
        new Claim("EmployeeId", employee.EmployeeId.ToString()) // optional extra claim
    };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],   
                audience: _config["JwtSettings:Audience"], 
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
