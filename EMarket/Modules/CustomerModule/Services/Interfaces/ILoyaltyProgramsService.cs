using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.CustomerModule.DTOs;

namespace EMarket.Modules.CustomerModule.Services.Interfaces
{
    public interface ILoyaltyProgramService
    {
        Task<List<LoyaltyProgramDTO>> GetAllLoyaltyAsync();
        Task<LoyaltyProgramDTO> GetLoyaltyByIdAsync(int id);
        Task<bool> CreateLoyaltyAsync(LoyaltyProgramDTO dto);
        Task<bool> UpdateLoyaltyAsync(LoyaltyProgramDTO dto);
        Task<bool> DeleteLoyaltyAsync(int id);
    }
}
