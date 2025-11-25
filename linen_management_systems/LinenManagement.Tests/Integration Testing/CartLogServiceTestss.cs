using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using linen_management_system.DTOModels;
using LinenManagement.Services;
using LinenManagementAPI.Data;
using LinenManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class CartLogServiceTestsss
{
    private readonly ApplicationDbContext _db;
    private readonly CartLogService _service;
    private readonly Mock<ILogger<CartLogService>> _logger;

    public CartLogServiceTestsss()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);
        _logger = new Mock<ILogger<CartLogService>>();
        _service = new CartLogService(_db, _logger.Object);

        SeedTestData();
    }

    // ----------------------------------------
    // 🚀 SEED DATA
    // ----------------------------------------
    private void SeedTestData()
    {
        _db.Carts.Add(new Cart { CartId = 1,Name= "Cart - Medium", Type = "CLEAN" });
        _db.Carts.Add(new Cart { CartId = 2, Name= "Cart - Small", Type = "SOILED" });

        _db.Linens.Add(new Linen { LinenId = 1, Name = "Bedsheet - Small" });
        _db.Linens.Add(new Linen { LinenId = 2, Name = "Bedsheet - Medium" });

        _db.SaveChanges();
    }

    // ----------------------------------------
    // ✅ TEST: CREATE CartLog
    // ----------------------------------------
    [Fact]
    public async Task UpsertCartLog_ShouldCreateCartLog()
    {
        var dto = new CartLogDto
        {
            ReceiptNumber = "R001",
            Comments = "Testing Create",
            ReportedWeight = 10,
            ActualWeight = 12,
            DateWeighed = DateTime.Now,
            CartId = 1,
            LocationId = 101,
            Linen = new List<LinenItemDto>
            {
                new LinenItemDto { LinenId = 1, Count = 5 }
            }
        };

        var result = await _service.UpsertCartLogAsync(dto, 999);

        Assert.NotNull(result);
        Assert.Equal("R001", result.ReceiptNumber);
        Assert.Single(result.Linen);
    }

    // ----------------------------------------
    // ✅ TEST: UPDATE CartLog
    // ----------------------------------------
    [Fact]
    public async Task UpsertCartLog_ShouldUpdateCartLog()
    {
        // Existing record
        var existing = new CartLog
        {
            CartLogId = 5,
            ReceiptNumber = "OLD",
            EmployeeId = 999,
            CartId = 1,
            LocationId = 100,
            DateWeighed = DateTime.Now
        };

        _db.CartLogs.Add(existing);
        _db.SaveChanges();

        var updateDto = new CartLogDto
        {
            CartLogId = 5,
            ReceiptNumber = "UPDATED",
            ReportedWeight = 20,
            ActualWeight = 18,
            Comments = "Updated comment",
            CartId = 1,
            LocationId = 100,
            Linen = new List<LinenItemDto>
            {
                new LinenItemDto { LinenId = 2, Count = 3 }
            }
        };

        var updated = await _service.UpsertCartLogAsync(updateDto, 999);

        Assert.Equal("UPDATED", updated.ReceiptNumber);
        Assert.Single(updated.Linen);
        Assert.Equal(2, updated.Linen[0].LinenId);
    }

    // ----------------------------------------
    // ✅ TEST: FETCH by ID
    // ----------------------------------------
    [Fact]
    public async Task GetCartLogById_ShouldReturnCorrectData()
    {
        var log = new CartLog
        {
            CartLogId = 10,
            ReceiptNumber = "R100",
            ReportedWeight = 5,
            CartId = 1,
            LocationId = 20,
            EmployeeId = 999,
            DateWeighed = DateTime.Now
        };

        _db.CartLogs.Add(log);
        _db.CartLogDetails.Add(new CartLogDetail { CartLogId = 10, LinenId = 1, Count = 2 });
        _db.SaveChanges();

        var result = await _service.GetCartLogByIdAsync(10);

        Assert.Equal("R100", result.ReceiptNumber);
        Assert.Single(result.Linen);
        Assert.Equal(1, result.Linen[0].LinenId);
    }

    // ----------------------------------------
    // ✅ TEST: GET list with filters
    // ----------------------------------------
    [Fact]
    public async Task GetCartLogsAsync_ShouldFilterByCartType()
    {
        _db.CartLogs.Add(new CartLog
        {
            CartLogId = 20,
            CartId = 1, // CLEAN
            EmployeeId = 500,
            LocationId = 100,
            DateWeighed = DateTime.Now
        });

        _db.CartLogs.Add(new CartLog
        {
            CartLogId = 21,
            CartId = 2, // DIRTY
            EmployeeId = 500,
            LocationId = 100,
            DateWeighed = DateTime.Now
        });

        _db.SaveChanges();

        var result = await _service.GetCartLogsAsync("CLEAN", null, null);

        Assert.Single(result);
        Assert.Equal(20, result.First().CartLogId);
    }

    // ----------------------------------------
    // ✅ TEST: DELETE
    // ----------------------------------------
    [Fact]
    public async Task DeleteCartLog_ShouldRemoveData()
    {
        var log = new CartLog
        {
            CartLogId = 30,
            CartId = 1,
            EmployeeId = 111,
            LocationId = 50,
            DateWeighed = DateTime.Now
        };

        _db.CartLogs.Add(log);
        _db.SaveChanges();

        await _service.DeleteCartLogAsync(30, 111);

        var check = await _db.CartLogs.FindAsync(30);
        Assert.Null(check);
    }

}
