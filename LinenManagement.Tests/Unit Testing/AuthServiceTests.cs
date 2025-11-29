using linen_management_system.Helper;
using linen_management_system.Interface;
using LinenManagementAPI.Data;
using LinenManagementAPI.DTOs;
using LinenManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

public class AuthServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public AuthServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"AuthTestDb_{Guid.NewGuid()}")
            .Options;
    }

    private AuthService CreateService(ApplicationDbContext context, Mock<IHelper> helperMock)
    {
        var loggerMock = new Mock<ILogger<AuthService>>();

        return new AuthService(context, helperMock.Object, loggerMock.Object);
    }

  
    [Fact]
    public async Task LoginAsync_Should_Return_Tokens_When_Credentials_Valid()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbOptions);
        var helperMock = new Mock<IHelper>();

        context.Employees.Add(new Employee
        {
            EmployeeId = 1,
            Email = "test@mail.com",
            Password = "hashedpwd"
        });
        await context.SaveChangesAsync();

        helperMock.Setup(h => h.VerifyPassword("123", "hashedpwd")).Returns(true);
        helperMock.Setup(h => h.GenerateAccessToken(It.IsAny<Employee>())).Returns("access123");
        helperMock.Setup(h => h.GenerateRefreshToken()).Returns("refresh123");

        var service = CreateService(context, helperMock);

        // Act
        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "test@mail.com",
            Password = "123"
        });

        // Assert
        Assert.Equal("access123", result.AccessToken);
        Assert.Equal("refresh123", result.RefreshToken);

        var updatedEmployee = await context.Employees.FirstAsync();
        Assert.Equal("refresh123", updatedEmployee.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_Password_Wrong()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbOptions);
        var helperMock = new Mock<IHelper>();

        context.Employees.Add(new Employee
        {
            EmployeeId = 1,
            Email = "test@mail.com",
            Password = "hashedpwd"
        });
        await context.SaveChangesAsync();

        helperMock.Setup(h => h.VerifyPassword("wrong", "hashedpwd")).Returns(false);

        var service = CreateService(context, helperMock);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginRequest
            {
                Email = "test@mail.com",
                Password = "wrong"
            }));
    }

  
    [Fact]
    public async Task RefreshTokenAsync_Should_Generate_New_Tokens()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbOptions);
        var helperMock = new Mock<IHelper>();

        context.Employees.Add(new Employee
        {
            EmployeeId = 1,
            Email = "emp@mail.com",
            RefreshToken = "oldrefresh"
        });
        await context.SaveChangesAsync();

        helperMock.Setup(h => h.GenerateAccessToken(It.IsAny<Employee>())).Returns("newAccess");
        helperMock.Setup(h => h.GenerateRefreshToken()).Returns("newRefresh");

        var service = CreateService(context, helperMock);

        // Act
        var result = await service.RefreshTokenAsync("oldrefresh");

        // Assert
        Assert.Equal("newAccess", result.AccessToken);
        Assert.Equal("newRefresh", result.RefreshToken);

        var emp = await context.Employees.FirstAsync();
        Assert.Equal("newRefresh", emp.RefreshToken);
    }

 
    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_When_Token_Invalid()
    {
        using var context = new ApplicationDbContext(_dbOptions);
        var helperMock = new Mock<IHelper>();

        var service = CreateService(context, helperMock);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.RefreshTokenAsync("doesnotexist"));
    }


    [Fact]
    public async Task LogoutAsync_Should_Clear_RefreshToken()
    {
        using var context = new ApplicationDbContext(_dbOptions);
        var helperMock = new Mock<IHelper>();

        context.Employees.Add(new Employee
        {
            EmployeeId = 1,
            Email = "emp@mail.com",
            RefreshToken = "oldtoken"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context, helperMock);

        // Act
        await service.LogoutAsync("emp@mail.com");

        // Assert
        var emp = await context.Employees.FirstAsync();
        Assert.Null(emp.RefreshToken);
    }


    [Fact]
    public async Task LogoutAsync_Should_Not_Fail_When_Employee_Not_Found()
    {
        using var context = new ApplicationDbContext(_dbOptions);
        var helperMock = new Mock<IHelper>();

        var service = CreateService(context, helperMock);

        // Should not throw any exception
        await service.LogoutAsync("notfound@mail.com");
    }
}
