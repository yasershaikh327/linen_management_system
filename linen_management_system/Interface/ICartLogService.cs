using linen_management_system.DTOModels;

namespace linen_management_system.Interface
{
    public interface ICartLogService
    {
        Task<CartLogDto> GetCartLogByIdAsync(int cartLogId);
        Task<List<CartLogDto>> GetCartLogsAsync(string cartType, int? locationId, int? employeeId);
        Task<CartLogDto> UpsertCartLogAsync(CartLogDto request, int currentEmployeeId);
        Task DeleteCartLogAsync(int cartLogId, int currentEmployeeId);
    }
}
