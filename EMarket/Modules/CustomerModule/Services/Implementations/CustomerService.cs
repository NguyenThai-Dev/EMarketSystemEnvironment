using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EMarket.Models;
using EMarket.Modules.CustomerModule.DTOs;
using EMarket.Modules.CustomerModule.Services.Interfaces;
using EMarket.Modules.DashboardModule.DTOs;

namespace EMarket.Modules.CustomerModule.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly EMarket_DBEntities _db;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public CustomerService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<CustomerDTO>> GetAllCustomerAsync()
        {
            return await _db.Customers
                .Select(x => new CustomerDTO
                {
                    CustomerId = x.customer_id,
                    FullName = x.full_name,
                    Email = x.email,
                    Phone = x.phone,
                    CustomerType = x.customer_type,
                    PointBalance = x.points_balance,
                    PointEarnedTotal = x.points_earned_total,
                    UserImg = x.user_img,
                    CreatedAt = x.created_at ?? defaultDate
                })
                .ToListAsync();
        }

        public async Task<List<CustomerDTO>> GetAllCustomerFilteredAsync(string keyword)
        {
            keyword = keyword?.Trim();

            var query = _db.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>
                    x.full_name.Contains(keyword) ||
                    x.email.Contains(keyword) ||
                    x.phone.Contains(keyword)
                );
            }

            return await query
                .OrderBy(x => x.full_name)
                .Select(x => new CustomerDTO
                {
                    CustomerId = x.customer_id,
                    FullName = x.full_name,
                    Email = x.email,
                    Phone = x.phone,
                    CustomerType = x.customer_type,
                    PointBalance = x.points_balance,
                    PointEarnedTotal = x.points_earned_total,
                    UserImg = x.user_img,
                    CreatedAt = x.created_at ?? defaultDate
                })
                .ToListAsync();
        }


        public async Task<CustomerDTO> GetCustomerByIdAsync(int id)
        {
            return await _db.Customers
                .Where(x => x.customer_id == id)
                .Select(x => new CustomerDTO
                {
                    CustomerId = x.customer_id,
                    FullName = x.full_name,
                    Email = x.email,
                    Phone = x.phone,
                    CustomerType = x.customer_type,
                    PointBalance = x.points_balance,
                    PointEarnedTotal = x.points_earned_total,
                    UserImg = x.user_img,
                    CreatedAt = x.created_at ?? defaultDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<int, string>> GetCustomerNameDictAsync(List<int> customerIds)
        {
            if (customerIds == null || !customerIds.Any())
                return new Dictionary<int, string>();

            return await _db.Customers
                .AsNoTracking()
                .Where(c => customerIds.Contains(c.customer_id))
                .Select(c => new
                {
                    c.customer_id,
                    c.full_name
                })
                .ToDictionaryAsync(x => x.customer_id, x => x.full_name);
        }

        public async Task<int> CreateCustomerAsync(CustomerCreateDTO dto, HttpPostedFileBase file)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new Customer
            {
                full_name = dto.FullName,
                email = dto.Email,
                phone = dto.Phone,
                customer_type = dto.CustomerType,
                created_at = DateTime.Now
            };

            _db.Customers.Add(entity);
            await _db.SaveChangesAsync(); // Lấy customer_id

            // ============================
            // XỬ LÝ ẢNH AVATAR
            // ============================
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string userFolder = HttpContext.Current.Server.MapPath($"~/Uploads/Users/{entity.customer_id}");
                    Directory.CreateDirectory(userFolder);

                    string ext = Path.GetExtension(file.FileName);
                    string fileName = "avatar" + ext; // Có thể đổi thành Guid nếu muốn
                    string fullPath = Path.Combine(userFolder, fileName);

                    file.SaveAs(fullPath);

                    entity.user_img = $"/Uploads/Users/{entity.customer_id}/{fileName}";
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi lưu ảnh avatar: " + ex.Message);
                }
            }

            return entity.customer_id;
        }


        public async Task<bool> UpdateCustomerAsync(CustomerUpdateDTO dto, HttpPostedFileBase file)
        {
            if (dto == null) return false;

            var entity = await _db.Customers.FirstOrDefaultAsync(c => c.customer_id == dto.CustomerId);
            if (entity == null) return false;

            entity.full_name = dto.FullName;
            entity.email = dto.Email;
            entity.phone = dto.Phone;
            entity.customer_type = dto.CustomerType;
            entity.updated_at = DateTime.Now;

            // ============================
            // XỬ LÝ ẢNH AVATAR MỚI
            // ============================
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string userFolder = HttpContext.Current.Server.MapPath($"~/Uploads/Users/{entity.customer_id}");
                    Directory.CreateDirectory(userFolder);

                    // Xóa avatar cũ nếu có
                    if (!string.IsNullOrEmpty(entity.user_img))
                    {
                        string oldPath = HttpContext.Current.Server.MapPath(entity.user_img);
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                    }

                    string ext = Path.GetExtension(file.FileName);
                    string fileName = "avatar" + ext;
                    string fullPath = Path.Combine(userFolder, fileName);

                    file.SaveAs(fullPath);
                    entity.user_img = $"/Uploads/Customers/{entity.customer_id}/{fileName}";
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi lưu ảnh avatar mới: " + ex.Message);
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var customer = await _db.Customers
                                            .FirstOrDefaultAsync(x => x.customer_id == id);

                    if (customer == null)
                        return false;

                    var addresses = await _db.CustomerAddresses
                                             .Where(x => x.customer_id == id)
                                             .ToListAsync();

                    if (addresses.Any())
                    {
                        _db.CustomerAddresses.RemoveRange(addresses);
                    }

                    _db.Customers.Remove(customer);

                    await _db.SaveChangesAsync();

                    DeleteCustomerImageFolder(id);

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw; // để controller/global handler xử lý
                }
            }
        }

        private void DeleteCustomerImageFolder(int customerId)
        {
            var rootPath = HttpContext.Current.Server.MapPath("~/Uploads/Customers/");
            var customerFolder = Path.Combine(rootPath, customerId.ToString());

            if (Directory.Exists(customerFolder))
            {
                Directory.Delete(customerFolder, recursive: true);
            }
        }


        public async Task<int> CountAllAsync()
        {
            return await _db.Customers.CountAsync();
        }

        public async Task<int> CountVipAsync()
        {
            return await _db.Customers
                .CountAsync(x => x.customer_type == "VIP");
        }

        public async Task<int> CountCreatedFromAsync(DateTime fromDate)
        {
            return await _db.Customers
                .CountAsync(x => x.created_at >= fromDate);
        }

        public async Task<int> CountCreatedInMonthAsync(DateTime fromDate, DateTime toDate)
        {
            return await _db.Customers
                .CountAsync(x => x.created_at >= fromDate && x.created_at < toDate);
        }

        public async Task<List<SegmentItemDTO>> GetCustomerSegmentsAsync()
        {
            return await _db.Customers
                .GroupBy(x => x.customer_type)
                .Select(g => new SegmentItemDTO
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<(int Month, int Count)>> GetCustomerCreatedByMonthAsync()
        {
            // Project to an anonymous type first, then materialize and convert to tuple in memory
            var result = await _db.Customers
                .GroupBy(x => x.created_at.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result.Select(x => (x.Month, x.Count)).ToList();
        }

        public async Task<List<CustomerRowDTO>> GetTopCustomersAsync(int top)
        {
            return await _db.Customers
                .OrderByDescending(x => x.points_earned_total)
                .Take(top)
                .Select(x => new CustomerRowDTO
                {
                    Name = x.full_name,
                    Email = x.email,
                    Phone = x.phone,
                    Type = x.customer_type,
                    Points = x.points_earned_total,
                    Avatar = x.user_img
                })
                .ToListAsync();
        }

        public async Task<string> GetCustomerEmailAsync(int customerId)
        {
            var customer = await _db.Customers
                .Where(x => x.customer_id == customerId)
                .Select(x => x.email)
                .FirstOrDefaultAsync();
            return customer;
        }
    }
}