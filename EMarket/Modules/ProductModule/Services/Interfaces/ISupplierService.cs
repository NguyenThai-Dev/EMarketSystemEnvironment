using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.ProductModule.DTOs;

namespace EMarket.Modules.ProductModule.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierDTO>> GetAllSupplierAsync();
        Task<List<SupplierDTO>> GetFilteredSupplierAsync(string name);
        Task<List<SupplierDTO>> GetAllSupplierByIdAsync(List<int> id);
        Task<SupplierDTO> GetSupplierByIdAsync(int id);
        Task<bool> CreateSupplierAsync(SupplierDTO dto);
        Task<bool> UpdateSupplierAsync(SupplierDTO dto);
        Task<bool> DeleteSupplierAsync(int id);
    }

}