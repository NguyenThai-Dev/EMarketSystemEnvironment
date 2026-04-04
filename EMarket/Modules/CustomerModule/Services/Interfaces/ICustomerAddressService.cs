using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.CustomerModule.DTOs;

namespace EMarket.Modules.CustomerModule.Services.Interfaces
{
    public interface ICustomerAddressService
    {
        Task<List<CustomerAddressDTO>> GetCustomerAddressAsync(int customerId);
        Task<CustomerAddressDTO> GetDefaultCustomerAddressAsync(int customerId);
        Task<CustomerAddressDTO> GetCustomerAddressByIdAsync(int id);
        Task<bool> CreateCustomerAddressAsync(CustomerAddressCreateDTO dto);
        Task<bool> UpdateCustomerAddressAsync(CustomerAddressUpdateDTO dto);
        Task<bool> DeleteCustomerAddressAsync(int id);
    }
}
