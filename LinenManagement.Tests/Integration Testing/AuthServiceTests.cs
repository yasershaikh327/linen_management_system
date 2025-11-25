using linen_management_system.Helper;
using LinenManagementAPI.Data;
using LinenManagementAPI.DTOs;
using LinenManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

public class AuthServiceTestss
{
    private ApplicationDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        return new ApplicationDbContext(options);
    }

    private Mock<IHelper> GetHelperMock()
    {
        var helperMock = new Mock<IHelper>();

        helperMock.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(true);

        helperMock.Setup(h => h.GenerateAccessToken(It.IsAny<Employee>()))
                  .Returns("dummy-access-token");

        helperMock.Setup(h => h.GenerateRefreshToken())
                  .Returns("dummy-refresh-token");

        return helperMock;
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        // Arrange
        var db = GetInMemoryDb();
        db.Employees.Add(new Employee
        {
            Email = "test@example.com",
            Password = "hashedpass"
        });
        await db.SaveChangesAsync();

        var helperMock = GetHelperMock();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var authService = new AuthService(db, helperMock.Object, loggerMock.Object);

        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var result = await authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("dummy-access-token", result.AccessToken);
        Assert.Equal("dummy-refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenValidRefreshToken()
    {
        // Arrange
        var db = GetInMemoryDb();
        db.Employees.Add(new Employee
        {
            Email = "test@example.com",
            RefreshToken = "old-refresh"
        });
        await db.SaveChangesAsync();

        var helperMock = GetHelperMock();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var authService = new AuthService(db, helperMock.Object, loggerMock.Object);

        // Act
        var result = await authService.RefreshTokenAsync("old-refresh");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("dummy-access-token", result.AccessToken);
        Assert.Equal("dummy-refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task LogoutAsync_ShouldClearRefreshToken()
    {
        // Arrange
        var db = GetInMemoryDb();
        db.Employees.Add(new Employee
        {
            Email = "test@example.com",
            RefreshToken = "abc123"
        });
        await db.SaveChangesAsync();

        var helperMock = GetHelperMock();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var authService = new AuthService(db, helperMock.Object, loggerMock.Object);

        // Act
        await authService.LogoutAsync("test@example.com");

        var user = await db.Employees.FirstAsync(e => e.Email == "test@example.com");

        // Assert
        Assert.Null(user.RefreshToken);
    }
}
