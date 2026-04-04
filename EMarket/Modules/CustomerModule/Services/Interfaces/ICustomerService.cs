using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using EMarket.Modules.CustomerModule.DTOs;
using EMarket.Modules.DashboardModule.DTOs;

namespace EMarket.Modules.CustomerModule.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> GetAllCustomerAsync();
        Task<List<CustomerDTO>> GetAllCustomerFilteredAsync(string keyword);
        Task<CustomerDTO> GetCustomerByIdAsync(int id);
        Task<int> CreateCustomerAsync(CustomerCreateDTO dto, HttpPostedFileBase file);
        Task<bool> UpdateCustomerAsync(CustomerUpdateDTO dto, HttpPostedFileBase file);
        Task<bool> DeleteCustomerAsync(int id);
        Task<Dictionary<int, string>> GetCustomerNameDictAsync(List<int> customerIds);

        // For Dashboard
        Task<int> CountAllAsync();
        Task<int> CountVipAsync();
        Task<int> CountCreatedFromAsync(DateTime fromDate);
        Task<int> CountCreatedInMonthAsync(DateTime fromDate, DateTime toDate);

        Task<List<SegmentItemDTO>> GetCustomerSegmentsAsync();
        Task<List<(int Month, int Count)>> GetCustomerCreatedByMonthAsync();

        Task<List<CustomerRowDTO>> GetTopCustomersAsync(int top);

        Task<string> GetCustomerEmailAsync(int customerId);
    }
}
