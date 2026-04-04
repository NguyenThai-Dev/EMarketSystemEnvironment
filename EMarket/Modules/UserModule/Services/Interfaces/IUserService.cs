using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using EMarket.Modules.DashboardModule.DTOs;
using EMarket.Modules.UserModule.DTOs;

namespace EMarket.Modules.UserModule.Services.Interfaces
{
    public interface IUserService
    {
        Task<int> CountActiveUsersAsync();
        Task<int> CountCreatedFromAsync(DateTime fromDate);
        Task<int> CountAllAsync();

        Task<List<string>> GetRecentActiveUserAvatarsAsync(int top);
        Task<List<(int Month, int Count)>> GetUsersCreatedByMonthAsync();

        Task<List<RoleStatItemDTO>> GetRoleStatisticsAsync();

        Task<List<string>> GetWarehouseManagerEmailsAsync();



        Task<List<CurrentUserDTO>> GetAllUsersAsync();
        Task<List<CurrentUserDTO>> GetFilteredUsersAsync(string keyword);
        Task<CurrentUserDTO> GetUserByIdAsync(int id);
        Task<List<CurrentUserDTO>> GetUsersByUserIdsAsync(List<int> userIds);
        Task<int> CreateUserAsync(CurrentUserDTO dto, HttpPostedFileBase file);
        Task<bool> UpdateUserAsync(CurrentUserDTO dto, HttpPostedFileBase file);
        Task<bool> DeleteUserAsync(int id);
        Task<Dictionary<int, CurrentUserDTO>> GetUserDictAsync();
        Task<Dictionary<int, CurrentUserDTO>> GetUserDictAsync(List<int> ids);
        Task<bool> UpdateUserEmailAsync(int userId, string newEmail);
    }

}
