using linen_management_system.Helper;
using linen_management_system.Interface;
using LinenManagementAPI.Data;
using LinenManagementAPI.DTOs;
using LinenManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IHelper _helper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, IHelper helper,ILogger<AuthService> logger)
    {
        _context = context;
        _helper = helper;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == request.Email);
        if (employee == null || !_helper.VerifyPassword(request.Password, employee.Password))
        {
            _logger.LogWarning($"Login failed for {request.Email}");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var accessToken = _helper.GenerateAccessToken(employee);
        var refreshToken = _helper.GenerateRefreshToken();

        employee.RefreshToken = refreshToken;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Login successful: {request.Email}");
        return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.RefreshToken == refreshToken);
        if (employee == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var newAccessToken = _helper.GenerateAccessToken(employee);
        var newRefreshToken = _helper.GenerateRefreshToken();

        employee.RefreshToken = newRefreshToken;
        await _context.SaveChangesAsync();

        return new AuthResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
    }

    public async Task LogoutAsync(string email)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
        if (employee != null)
        {
            employee.RefreshToken = null;
            await _context.SaveChangesAsync();
        }
    }

    
}
