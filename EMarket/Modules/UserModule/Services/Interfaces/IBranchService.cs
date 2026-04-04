using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.UserModule.DTOs;

namespace EMarket.Modules.UserModule.Services.Interfaces
{
    public interface IBranchService
    {
        Task<List<BranchDTO>> GetAllBranchesAsync();
        Task<List<BranchDTO>> GetFilteredBranchesAsync(string branchName);
        Task<List<BranchDTO>> GetBranchByIdsAsync(List<int> ids);
        Task<BranchDTO> GetBranchByIdAsync(int id);
        Task<int> CreateBranchAsync(BranchDTO dto);
        Task<bool> UpdateBranchAsync(BranchDTO dto);
        Task<bool> DeleteBranchAsync(int id);
        Task<List<BranchDTO>> GetNearestBranchAsync(double lat, double lng, double maxDistanceKm);
        Task<Dictionary<int, BranchDTO>> GetBranchDictAsync();
    }
}
