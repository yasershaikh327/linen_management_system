using linen_management_system.DTOModels;
using LinenManagement.Services;
using LinenManagementAPI.Data;
using LinenManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CartLogServiceTestss
{
    private DbContextOptions<ApplicationDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"CartLogTestDb_{Guid.NewGuid()}")
            .Options;

    private CartLogService CreateService(ApplicationDbContext context)
    {
        var logger = new Mock<ILogger<CartLogService>>();
        return new CartLogService(context, logger.Object);
    }

    [Fact]
    public async Task GetCartLogByIdAsync_Should_Return_Dto()
    {
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        // Arrange
        context.Carts.Add(new Cart { CartId = 1, Name= "Cart - Medium", Type = "CLEAN" });
        context.Linens.Add(new Linen { LinenId = 10, Name = "Bedsheet - Medium" });
        context.CartLogs.Add(new CartLog { CartLogId = 5, CartId = 1, EmployeeId = 1 });
        context.CartLogDetails.Add(new CartLogDetail
        {
            CartLogId = 5,
            LinenId = 10,
            Count = 3
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        // Act
        var result = await service.GetCartLogByIdAsync(5);

        // Assert
        Assert.Equal(5, result.CartLogId);
        Assert.Single(result.Linen);
        Assert.Equal("Bedsheet - Medium", result.Linen.First().Name);
    }

    [Fact]
    public async Task GetCartLogByIdAsync_Should_Throw_When_NotFound()
    {
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);
        var service = CreateService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetCartLogByIdAsync(999));
    }


    [Fact]
    public async Task UpsertCartLogAsync_Should_Create_New_Record()
    {
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);
        var service = CreateService(context);

        var dto = new CartLogDto
        {
            ReceiptNumber = "R001",
            CartId = 1,
            LocationId = 1,
            Linen = new List<LinenItemDto>
            {
                new LinenItemDto { LinenId = 5, Count = 2 }
            }
        };

        context.Linens.Add(new Linen { LinenId = 5, Name = "Cart - Medium" });
        context.Carts.Add(new Cart { CartId = 1, Name = "Cart - Medium", Type = "CLEAN" });
        await context.SaveChangesAsync();

        var result = await service.UpsertCartLogAsync(dto, currentEmployeeId: 99);

        Assert.Equal("R001", result.ReceiptNumber);
        Assert.Single(result.Linen);
        Assert.Equal(5, result.Linen.First().LinenId);
        Assert.Equal(99, result.EmployeeId);
    }

 
    [Fact]
    public async Task UpsertCartLogAsync_Should_Update_Record()
    {
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        context.Carts.Add(new Cart { CartId = 1,Name= "Cart - Extra Large", Type = "CLEAN" });
        context.Linens.Add(new Linen { LinenId = 50, Name = "Bedsheet - Medium" });

        context.CartLogs.Add(new CartLog
        {
            CartLogId = 7,
            CartId = 1,
            EmployeeId = 99
        });

        context.CartLogDetails.Add(new CartLogDetail
        {
            CartLogDetailId = 1,
            CartLogId = 7,
            LinenId = 50,
            Count = 1
        });

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var request = new CartLogDto
        {
            CartLogId = 7,
            ReceiptNumber = "UPDATED",
            CartId = 1,
            Linen = new List<LinenItemDto>
            {
                new LinenItemDto { LinenId = 50, Count = 10 }
            }
        };

        var updated = await service.UpsertCartLogAsync(request, 99);

        Assert.Equal("UPDATED", updated.ReceiptNumber);
        Assert.Single(updated.Linen);
        Assert.Equal(10, updated.Linen.First().Count);
    }


    [Fact]
    public async Task UpsertCartLogAsync_Should_Throw_When_Unauthorized()
    {
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        context.CartLogs.Add(new CartLog { CartLogId = 3, EmployeeId = 100 });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var dto = new CartLogDto { CartLogId = 3 };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UpsertCartLogAsync(dto, currentEmployeeId: 999));
    }

    [Fact]
    public async Task GetCartLogsAsync_Should_Filter_Records()
    {
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        context.Carts.Add(new Cart { CartId = 1, Name= "Cart - Small", Type = "CLEAN" });
        context.Carts.Add(new Cart { CartId = 2, Name= "Cart - Medium", Type = "SOILED" });

        context.CartLogs.Add(new CartLog { CartLogId = 1, CartId = 1, EmployeeId = 10, LocationId = 5 });
        context.CartLogs.Add(new CartLog { CartLogId = 2, CartId = 2, EmployeeId = 10, LocationId = 5 });
        context.CartLogs.Add(new CartLog { CartLogId = 3, CartId = 1, EmployeeId = 11, LocationId = 5 });

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetCartLogsAsync("Clean", locationId: 5, employeeId: 10);

        Assert.Single(result);
        Assert.Equal(1, result.First().CartLogId);
    }

 
    [Fact]
    public async Task DeleteCartLogAsync_Should_Delete_Record()
    {
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        context.CartLogs.Add(new CartLog { CartLogId = 15, EmployeeId = 99 });
        context.CartLogDetails.Add(new CartLogDetail { CartLogId = 15, LinenId = 5, Count = 1 });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        await service.DeleteCartLogAsync(15, 99);

        Assert.Empty(context.CartLogs);
        Assert.Empty(context.CartLogDetails);
    }

  
    [Fact]
    public async Task DeleteCartLogAsync_Should_Throw_When_Unauthorized()
    {
        var options = CreateOptions();
        using var context = new ApplicationDbContext(options);

        context.CartLogs.Add(new CartLog { CartLogId = 20, EmployeeId = 100 });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.DeleteCartLogAsync(20, currentEmployeeId: 999));
    }
}
