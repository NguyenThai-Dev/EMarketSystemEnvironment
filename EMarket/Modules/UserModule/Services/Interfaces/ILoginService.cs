using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.UserModule.DTOs;

namespace EMarket.Modules.UserModule.Services.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponseDTO> LoginAsync(string emailOrUsername, string password);
        Task<List<CurrentUserDTO>> GetAllUsersByIdsAsync(List<int> ids);
        int? GetCurrentUserId();
        Task<CurrentUserDTO> GetUserByIdAsync(int userId);
        Task<int> GetCurrentUserBranch();
        Task RequestOtpAsync(string email);
        Task ResetPasswordAsync(string email, string otp, string newPassword);
        Task<List<string>> GetEmailsByRoleAsync(int roleId);
        Task<LoginResponseDTO> LoginByEmailAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otp);
    }
}