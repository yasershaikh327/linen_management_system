using linen_management_system.DTOModels;
using linen_management_system.Interface;
using LinenManagementAPI.DTOs;

namespace linen_management_system.Interface
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string email);
    }

}
