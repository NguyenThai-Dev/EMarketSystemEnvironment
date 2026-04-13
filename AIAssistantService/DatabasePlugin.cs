using System.ComponentModel;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;

namespace AIAssistantService.Plugins
{
    public class DatabasePlugin
    {
        private readonly string _connectionString;

        public DatabasePlugin(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Đổi return type từ string sang IEnumerable<dynamic> để Controller dễ xử lý
        [KernelFunction, Description("Thực thi truy vấn SQL Server.")]
        public async Task<IEnumerable<dynamic>> ExecuteQueryAsync(string sql)
        {
            Console.WriteLine($"[AI SQL EXECUTE]: {sql}");

            // Basic Security check
            // Sửa dòng 24 lại như sau:
            var trimmedSql = sql.TrimStart();
            bool isSafe = trimmedSql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                          trimmedSql.StartsWith("WITH", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(sql) || !isSafe)
            {
                throw new ArgumentException("Chỉ cho phép câu lệnh SELECT hoặc WITH để đảm bảo an toàn.");
            }

            // KHÔNG dùng try-catch ở đây. Hãy để lỗi "nổ" ra ngoài 
            // để Controller bắt được và ghi vào bảng Learning.
            using var connection = new SqlConnection(_connectionString);

            // Dùng Dapper lấy dữ liệu thô. Set Timeout để tránh treo.
            var results = await connection.QueryAsync(sql, commandTimeout: 30);
            return results;
        }

        public async Task<string> GetAppBaseUrl()
        {
            //// Query thẳng vào View, không động vào bảng gốc
            //string sql = "SELECT config_value FROM View_AIAssistant_Configs";

            //using var connection = new SqlConnection(_connectionString);
            //var value = await connection.QueryFirstOrDefaultAsync<string>(sql);

            //return value ?? "https://localhost:default/";

            return "https://localhost:44339/";
        }
    }
}