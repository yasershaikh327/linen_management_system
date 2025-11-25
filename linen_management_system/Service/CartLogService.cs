using linen_management_system.DTOModels;
using linen_management_system.Interface;
using LinenManagementAPI.Data;
using LinenManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LinenManagement.Services
{
    public class CartLogService : ICartLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartLogService> _logger;

        public CartLogService(ApplicationDbContext context, ILogger<CartLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartLogDto> GetCartLogByIdAsync(int cartLogId)
        {
            var cartLog = await _context.CartLogs.FindAsync(cartLogId);
            if (cartLog == null)
                throw new KeyNotFoundException($"CartLog {cartLogId} not found");

            var cart = await _context.Carts.FindAsync(cartLog.CartId);

            var dto = new CartLogDto
            {
                CartLogId = cartLog.CartLogId,
                ReceiptNumber = cartLog.ReceiptNumber ?? string.Empty,
                ReportedWeight = cartLog.ReportedWeight??0,
                ActualWeight = cartLog.ActualWeight ?? 0,
                Comments = cartLog.Comments ?? string.Empty,
                DateWeighed = cartLog.DateWeighed,
                CartId = cartLog.CartId,
                LocationId = cartLog.LocationId,
                EmployeeId = cartLog.EmployeeId,
                Linen = new List<LinenItemDto>()
            };

            // Only return linen if cart type is CLEAN
            if (cart?.Type?.Equals("Clean", StringComparison.OrdinalIgnoreCase) == true)
            {
                var details = await _context.CartLogDetails
                    .Where(d => d.CartLogId == cartLogId)
                    .ToListAsync();

                foreach (var detail in details)
                {
                    var linen = await _context.Linens.FindAsync(detail.LinenId);
                    if (linen != null)
                    {
                        dto.Linen.Add(new LinenItemDto
                        {
                            CartLogDetailId = detail.CartLogDetailId,
                            LinenId = detail.LinenId,
                            Name = linen.Name,
                            Count = detail.Count
                        });
                    }
                }
            }

            return dto;
        }

        //public async Task<CartLogDto> UpsertCartLogAsync(CartLogDto request, int currentEmployeeId)
        //{
        //    CartLog cartLog;

        //    // Check if updating (CartLogId > 0 means existing record)
        //    if (request.CartLogId > 0)
        //    {
        //        // Fetch the existing CartLog
        //        cartLog = await _context.CartLogs.FindAsync(request.CartLogId);
        //        if (cartLog == null)
        //            throw new KeyNotFoundException("CartLog not found");

        //        // Verify authorization
        //        if (cartLog.EmployeeId != currentEmployeeId)
        //            throw new UnauthorizedAccessException("Only creator can edit");

        //        // Update fields
        //        cartLog.ReceiptNumber = request.ReceiptNumber ?? string.Empty;
        //        cartLog.Comments = request.Comments ?? string.Empty;
        //        cartLog.ReportedWeight = request.ReportedWeight;
        //        cartLog.ActualWeight = request.ActualWeight;
        //        cartLog.DateWeighed = request.DateWeighed;
        //        cartLog.CartId = request.CartId;
        //        cartLog.LocationId = request.LocationId;

        //        // Remove old linen details
        //        var oldDetails = await _context.CartLogDetails
        //            .Where(d => d.CartLogId == cartLog.CartLogId)
        //            .ToListAsync();
        //        _context.CartLogDetails.RemoveRange(oldDetails);
        //    }
        //    else // Create new
        //    {
        //        cartLog = new CartLog
        //        {
        //            ReceiptNumber = request.ReceiptNumber ?? string.Empty,
        //            Comments = request.Comments ?? string.Empty,
        //            ReportedWeight = request.ReportedWeight,
        //            ActualWeight = request.ActualWeight,
        //            DateWeighed = request.DateWeighed,
        //            CartId = request.CartId,
        //            LocationId = request.LocationId,
        //            EmployeeId = currentEmployeeId
        //        };
        //        _context.CartLogs.Add(cartLog);
        //    }

        //    // Save CartLog (create or update)
        //    await _context.SaveChangesAsync();

        //    // Add new linen details
        //    if (request.Linen?.Count > 0)
        //    {
        //        var linenDetails = request.Linen.Select(l => new CartLogDetail
        //        {
        //            CartLogId = cartLog.CartLogId,
        //            LinenId = l.LinenId,
        //            Count = l.Count
        //        }).ToList();

        //        _context.CartLogDetails.AddRange(linenDetails);
        //        await _context.SaveChangesAsync();
        //    }

        //    return await GetCartLogByIdAsync(cartLog.CartLogId);
        //}
        public async Task<CartLogDto> UpsertCartLogAsync(CartLogDto request, int currentEmployeeId)
        {
            CartLog cartLog = null;

            if (request.CartLogId > 0)
            {
                // Fetch existing CartLog
                cartLog = await _context.CartLogs
                    .Include(c => c.CartLogDetail)
                    .FirstOrDefaultAsync(c => c.CartLogId == request.CartLogId);

                if (cartLog == null)
                    throw new KeyNotFoundException("CartLog not found");

                if (cartLog.EmployeeId != currentEmployeeId)
                    throw new UnauthorizedAccessException("Only creator can edit");

                // Update fields
                cartLog.ReceiptNumber = request.ReceiptNumber ?? "";
                cartLog.Comments = request.Comments ?? "";
                cartLog.ReportedWeight = request.ReportedWeight;
                cartLog.ActualWeight = request.ActualWeight;
                cartLog.DateWeighed = request.DateWeighed;
                cartLog.CartId = request.CartId;
                cartLog.LocationId = request.LocationId;

                // Remove old linen details
                if (cartLog.CartLogDetail.Any())
                    _context.CartLogDetails.RemoveRange(cartLog.CartLogDetail);
            }
            else
            {
                // Create new CartLog
                cartLog = new CartLog
                {
                    ReceiptNumber = request.ReceiptNumber ?? "",
                    Comments = request.Comments ?? "",
                    ReportedWeight = request.ReportedWeight,
                    ActualWeight = request.ActualWeight,
                    DateWeighed = request.DateWeighed,
                    CartId = request.CartId,
                    LocationId = request.LocationId,
                    EmployeeId = currentEmployeeId
                };
                await _context.CartLogs.AddAsync(cartLog);
            }

            await _context.SaveChangesAsync();

            // Add linen details if provided
            if (request.Linen?.Count > 0)
            {
                var linenDetails = request.Linen.Select(l => new CartLogDetail
                {
                    CartLogId = cartLog.CartLogId,
                    LinenId = l.LinenId,
                    Count = l.Count
                });

                await _context.CartLogDetails.AddRangeAsync(linenDetails);
                await _context.SaveChangesAsync();
            }

            return await GetCartLogByIdAsync(cartLog.CartLogId);
        }



        public async Task<List<CartLogDto>> GetCartLogsAsync(string cartType, int? locationId, int? employeeId)
        {
            var query = _context.CartLogs.AsQueryable();

            if (!string.IsNullOrEmpty(cartType))
            {
                var cartIds = await _context.Carts
                    .Where(c => c.Type.ToLower() == cartType.ToLower())
                    .Select(c => c.CartId)
                    .ToListAsync();
                query = query.Where(cl => cartIds.Contains(cl.CartId));
            }

            if (locationId.HasValue)
                query = query.Where(cl => cl.LocationId == locationId);

            if (employeeId.HasValue)
                query = query.Where(cl => cl.EmployeeId == employeeId);

            var cartLogs = await query.OrderByDescending(cl => cl.DateWeighed).ToListAsync();
            var responses = new List<CartLogDto>();

            foreach (var cartLog in cartLogs)
                responses.Add(await GetCartLogByIdAsync(cartLog.CartLogId));

            return responses;
        }

        public async Task DeleteCartLogAsync(int cartLogId, int currentEmployeeId)
        {
            var cartLog = await _context.CartLogs.FindAsync(cartLogId);
            if (cartLog == null)
                throw new KeyNotFoundException("CartLog not found");

            if (cartLog.EmployeeId != currentEmployeeId)
                throw new UnauthorizedAccessException("Only creator can delete");

            var details = _context.CartLogDetails.Where(d => d.CartLogId == cartLogId);
            _context.CartLogDetails.RemoveRange(details);
            _context.CartLogs.Remove(cartLog);

            await _context.SaveChangesAsync();
        }
    }
}
