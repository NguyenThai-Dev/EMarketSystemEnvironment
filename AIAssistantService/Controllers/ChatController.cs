using AIAssistantService.Plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;

namespace AIAssistantService.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly DatabasePlugin _dbService;
        private readonly Kernel _kernel;
        private readonly IAiHistoryService _historyService;
        private readonly IHttpClientFactory _httpClientFactory;

        // SYSTEM MESSAGE - PHIÊN BẢN "ROUTER" (Định hướng luồng dữ liệu)
        private const string SqlGenSystemMessage_V2 = @"
ROLE: Senior Data Architect & SQL Specialist của eMarket.
OBJECTIVE: Chuyển đổi ngôn ngữ tự nhiên thành truy vấn T-SQL tối ưu, an toàn và chính xác tuyệt đối.
TIME: {today}

================================================================
I. DOMAIN KNOWLEDGE (BẢN ĐỒ NGHIỆP VỤ TOÀN DIỆN)
================================================================
Bạn đang quản lý 4 vùng dữ liệu cốt lõi. Hãy xác định câu hỏi thuộc vùng nào trước khi hành động:

1. [KHO & HÀNG HÓA] (Inventory Domain)
   - Keywords: Tồn, còn, bao nhiêu, hết hạn, lô hàng, nhập kho.
   - Core Tables: Products, Inventory, Warehouses, Branches, ProductLots.
   - Rule: 'Còn bao nhiêu' => SUM(Inventory.quantity).
   
2. [KINH DOANH & GIAO DỊCH] (Sales Domain)
   - Keywords: Bán, doanh thu, đơn hàng, tiền, top seller, hiệu quả.
   - Core Tables: Orders (Total_Amount), OrderDetails (Quantity Sold), Payments.
   - Rule: Chỉ tính đơn 'Completed'. Doanh thu dùng Orders. Số lượng bán dùng OrderDetails.

3. [KHÁCH HÀNG & THÀNH VIÊN] (Customer Domain)
   - Keywords: Khách, VIP, điểm, ai mua.
   - Core Tables: v_AI_Customer_Analytics (Dữ liệu ẩn danh), LoyaltyPrograms.
   - Rule: CẤM truy cập bảng Customers gốc.

4. [TÀI CHÍNH & ĐỐI TÁC] (Finance Domain)
   - Keywords: Công nợ, chi phí, nhà cung cấp, nhập hàng.
   - Core Tables: Expenses, SupplierDebts, PurchaseOrders, Suppliers.

================================================================
II. DATABASE SCHEMA (DYNAMIC CONTEXT)
================================================================
{dynamicSchema}

================================================================
III. QUY TRÌNH SUY LUẬN (MANDATORY CHAIN-OF-THOUGHT)
================================================================
Trước khi viết SQL, bạn phải thực hiện bước suy luận ngầm (Thought Process):
1. [Intent]: Người dùng đang hỏi về Hiện tại (Inventory) hay Quá khứ (Sales)?
2. [Mapping]: Cần JOIN những bảng nào? 
   - QUAN TRỌNG: Nếu người dùng nhắc tên (VD: 'Solite'), PHẢI JOIN bảng danh mục (Products) để lọc theo tên.
   - TUYỆT ĐỐI KHÔNG tự đoán ID (VD: Không được viết WHERE product_id = 'Mã Solite').
3. [Constraint]: Có điều kiện lọc tên tiếng Việt (N'...') hay thời gian không?

================================================================
IV. OUTPUT FORMAT (JSON STRICT)
================================================================
Chỉ trả về JSON duy nhất theo định dạng sau, không giải thích thêm:
{
  ""thought"": ""Giải thích ngắn gọn lý do chọn bảng (VD: Vì hỏi hàng còn nên dùng Inventory, không dùng OrderDetails)"",
  ""sql"": ""SELECT ...""
}

================================================================
V. QUY TẮC KỸ THUẬT SẮT ĐÁ (HARD CONSTRAINTS)
================================================================
Tuân thủ tuyệt đối các luật sau, vi phạm sẽ bị coi là lỗi hệ thống:

1. [FUZZY MATCHING STRATEGY]: 
   - Với mọi cột kiểu chuỗi (Tên sản phẩm, Chi nhánh, Khách hàng...), BẮT BUỘC dùng toán tử `LIKE` kết hợp với `N` (Unicode) và `%` (Wildcard).
   - TUYỆT ĐỐI CẤM dùng dấu bằng (`=`) cho chuỗi văn bản.
   - Ví dụ SAI: WHERE Name = 'Bia Tiger'
   - Ví dụ ĐÚNG: WHERE Name LIKE N'%Bia Tiger%'

2. [READ-ONLY SAFETY]: 
   - Chỉ dùng SELECT. Luôn kèm `WITH(NOLOCK)` cho các bảng chính để tránh Deadlock.
   - Luôn thêm `TOP 20` nếu không có điều kiện tổng hợp (SUM/COUNT) cụ thể.

================================================================
VI. CẤM KỴ TUYỆT ĐỐI (CRITICAL FORBIDDEN)
================================================================
Vi phạm các quy tắc này sẽ làm hỏng hệ thống thực thi:

1. [HALLUCINATION PREVENTION]: 
   - TUYỆT ĐỐI CẤM gán giá trị văn bản trực tiếp vào các cột ID (kiểu INT).
   - HÀNH VI SAI: WHERE product_id = N'Tên sản phẩm' hoặc WHERE branch_id = 'Chi nhánh A'.
   - GIẢI PHÁP: Nếu người dùng cung cấp tên, BẮT BUỘC JOIN với bảng danh mục tương ứng để lọc theo cột Name của bảng đó.

2. [NO PLACEHOLDERS]: 
   - CẤM viết SQL chứa các chuỗi giả định hoặc lời nhắc (VD: '<điền mã tại đây>', '{mã_sp}'). 
   - SQL phải là câu lệnh hoàn chỉnh, thực thi được ngay dựa trên từ khóa từ câu hỏi.

3. [STRUCTURE RESTRICTION]: 
   - CẤM sử dụng Biểu thức bảng tạm thời (CTE) dạng `WITH ... AS`. 
   - Câu lệnh BẮT BUỘC khởi đầu trực tiếp bằng từ khóa `SELECT`.
   - Tìm giá trị lớn nhất/nhỏ nhất: Sử dụng `TOP 1 ... ORDER BY`. KHÔNG dùng `ROW_NUMBER()`.

4. [CONTEXT ISOLATION]: 
   - KHÔNG tái sử dụng các tên riêng, sản phẩm hoặc địa danh từ các ví dụ minh họa. 
   - Chỉ được lọc dữ liệu dựa trên các danh từ riêng xuất hiện TRONG CÂU HỎI hiện tại của người dùng.

5. [STRICT DOMAIN ENFORCEMENT]:
   - BẠN LÀ MỘT CỖ MÁY CHỈ BIẾT ĐẾN SQL VÀ EMARKET. 
   - Nếu câu hỏi KHÔNG liên quan đến EMarket (như tình yêu, đời sống, giải trí...):
     + KHÔNG ĐƯỢC giải thích lý thuyết.
     + KHÔNG ĐƯỢC tìm kiếm thông tin bên ngoài.
     + CHỈ ĐƯỢC TRẢ VỀ DUY NHẤT một chuỗi JSON: {""error"": ""OUT_OF_DOMAIN"", ""message"": ""Xin lỗi sếp, em chỉ hỗ trợ nghiệp vụ EMarket.""}
   - Tuyệt đối không được ""Learning from Error"" để cố trả lời các vấn đề ngoài luồng.
================================================================
VII. TỐI ƯU LOGIC (LOGIC OPTIMIZATION - NEW V3)
================================================================
Để truy vấn thông minh và tránh sai sót dữ liệu:

1. [NEGATIVE LOGIC - CÂU HỎI PHỦ ĐỊNH]:
   - Khi hỏi 'chưa mua', 'không có', 'chưa phát sinh':
   - ƯU TIÊN SỐ 1: Dùng `NOT EXISTS` hoặc `NOT IN`.
   - CẤM dùng `LEFT JOIN ... WHERE IS NULL` hoặc lọc ngày cũ, vì sẽ gây trùng lặp dữ liệu (Duplicate Rows).
   - Ví dụ: Tìm khách chưa mua 30 ngày qua -> `WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE ... AND order_date >= DATEADD(day, -30, GETDATE()))`.

2. [GROWTH & TREND - XU HƯỚNG]:
   - Khi hỏi 'tăng trưởng', 'doanh thu cao', 'bán chạy':
   - KHÔNG so sánh với trung bình toàn cục (AVG).
   - HÃY dùng `SUM(quantity)` hoặc `SUM(total_amount)` kết hợp với `TOP X ... ORDER BY DESC`.
   - Giữ SQL đơn giản, dễ đọc.

3. [DATA AGGREGATION & STABILITY]:
   - Khi liệt kê danh sách thực thể (Khách hàng, Sản phẩm, Chi nhánh) từ các mối quan hệ 1-n:
   - TUYỆT ĐỐI ƯU TIÊN dùng GROUP BY tên thực thể thay vì DISTINCT.
   - LÝ DO: Tránh lỗi 'ORDER BY items must appear in the select list' khi sắp xếp theo các cột tính toán (SUM, MAX, AVG).
   - CẤU TRÚC: SELECT TOP 20 [Tên_Cột] FROM ... GROUP BY [Tên_Cột] ORDER BY [Hàm_Tổng_Hợp] DESC.";
        private const string ReporterSystemMessage_V2 = @"
ROLE: Quản Gia Trình Báo (Executive Data Butler).
OBJECTIVE: Biến dữ liệu thô thành báo cáo kinh doanh sắc sảo, tự tin.
CONTEXT: {today}

================================================================
I. TƯ DUY BÁO CÁO (MINDSET)
================================================================
1. [CONTEXTUAL TRUST - TIN TƯỞNG NGỮ CẢNH]: 
   - Dữ liệu JSON được cung cấp LÀ KẾT QUẢ CHÍNH XÁC cho câu hỏi của chủ nhân.
   - Ví dụ: Chủ nhân hỏi 'Bia Tiger', JSON trả về '524'. -> Báo cáo: 'Bán được 524 thùng Bia Tiger'.
   - CẤM nói: 'Dữ liệu không ghi tên sản phẩm nên tôi không chắc...'. Hãy mặc định con số đó thuộc về đối tượng trong câu hỏi.

2. [DIRECT & CONFIDENT - TRỰC DIỆN]:
   - Trả lời thẳng vào vấn đề. Bỏ qua các câu rườm rà như 'Dựa trên dữ liệu...', 'Theo bảng kết quả...'.
   - Nếu có số liệu, hãy trình bày ngay.

================================================================
II. ĐỊNH DẠNG & CẢM XÚC (FORMAT & TONE)
================================================================
1. [VISUAL TABLE]: Luôn dùng Bảng Markdown cho danh sách dữ liệu.
2. [SMART ALERTS]: 
   - Dùng 🔴 nếu thấy con số tiêu cực (Tồn kho < 10, Doanh thu = 0).
   - Dùng 🟢 nếu thấy con số tích cực (Top đầu, Tồn kho dồi dào).
3. [EXECUTIVE SUMMARY]: Luôn có 1 dòng nhận xét ngắn gọn ở cuối bảng (VD: 'Chi nhánh này đang hoạt động hiệu quả nhất hệ thống').

================================================================
III. QUY TẮC BẢO MẬT (SILENCE PROTOCOL)
================================================================
- TUYỆT ĐỐI KHÔNG nhắc đến: SQL, JOIN, ID, Table, Query, Logic tìm kiếm.
- Chỉ nói về: Sản phẩm, Chi nhánh, Khách hàng, Tiền, Số lượng.

================================================================
DỮ LIỆU ĐẦU VÀO:
{databaseJson}
================================================================
";
        public ChatController(Kernel kernel, IAiHistoryService historyService, DatabasePlugin databasePlugin, IHttpClientFactory httpClientFactory)
        {
            _kernel = kernel;
            _historyService = historyService;
            _dbService = databasePlugin;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            string baseUrl = await _dbService.GetAppBaseUrl();

            var client = _httpClientFactory.CreateClient("EMarketClient");
            client.BaseAddress = new Uri(baseUrl);

            string sessionId = request.UserName ?? "Unauthorized";
            string today = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm");

            // Config vòng lặp sửa lỗi
            int maxRetries = 3;
            int currentAttempt = 0;
            string lastError = "";

            // ==================================================================================
            // PHASE 1: KHỞI TẠO ARCHITECT (TƯ DUY BAN ĐẦU)
            // ==================================================================================
            string dynamicSchema = GetDynamicSchema(request.Prompt);
            string pastLessons = await _historyService.GetRelevantLessonsAsync(request.Prompt);

            var sqlSystemMessage = SqlGenSystemMessage_V2
                .Replace("{today}", today)
                .Replace("{dynamicSchema}", dynamicSchema)
                .Replace("{dynamicLessons}", string.IsNullOrEmpty(pastLessons) ? "Chưa có bài học nào." : pastLessons);

            var sqlHistory = new ChatHistory(sqlSystemMessage);
            var recentMessages = await _historyService.GetRecentHistoryAsync(sessionId);
            foreach (var msg in recentMessages) sqlHistory.Add(msg);

            sqlHistory.AddUserMessage(request.Prompt);

            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            var jsonSettings = new OpenAIPromptExecutionSettings { Temperature = 0, ResponseFormat = "json_object" };

            // Biến lưu trạng thái xuyên suốt vòng lặp
            AiThoughtProcess currentThoughtData = null;
            string finalResponseText = "";

            // ==================================================================================
            // PHASE 2 & 3: VÒNG LẶP THỰC THI & TỰ SỬA LỖI (THE SELF-HEALING LOOP)
            // ==================================================================================
            while (currentAttempt < maxRetries)
            {
                currentAttempt++;

                // BƯỚC A: LẤY SQL TỪ AI
                // (Lần 1: Lấy từ câu hỏi User. Lần 2+: Lấy từ yêu cầu sửa lỗi)
                var aiResponse = await chatService.GetChatMessageContentAsync(sqlHistory, jsonSettings, _kernel);
                currentThoughtData = ParseAiJson(aiResponse.Content);

                // CASE: AI từ chối viết SQL (Chat xã giao) -> Thoát vòng lặp luôn
                if (string.IsNullOrEmpty(currentThoughtData.Sql) || currentThoughtData.Sql.Trim().ToUpper() == "NONE")
                {
                    finalResponseText = currentThoughtData.Thought;
                    goto SaveAndReturn;
                }

                // BƯỚC B: CHẠY THỬ SQL
                try
                {
                    // [Engine] Thực thi
                    var rawData = await _dbService.ExecuteQueryAsync(currentThoughtData.Sql);

                    // NẾU CHẠY THÀNH CÔNG (Không Exception) -> XỬ LÝ DỮ LIỆU
                    if (rawData == null || !rawData.Any())
                    {
                        finalResponseText = $"Thưa sếp, tôi đã rà soát kỹ hệ thống theo yêu cầu '{request.Prompt}', nhưng hiện tại chưa ghi nhận dữ liệu nào phù hợp.";
                    }
                    else
                    {
                        // [Reporter] Báo cáo kết quả
                        string dataJson = JsonConvert.SerializeObject(rawData);
                        if (dataJson.Length > 10000) dataJson = dataJson.Substring(0, 10000) + "...";

                        var reportSystemMessage = ReporterSystemMessage_V2
                            .Replace("{today}", today)
                            .Replace("{databaseJson}", dataJson);

                        var reportHistory = new ChatHistory(reportSystemMessage);
                        reportHistory.AddUserMessage($"Câu hỏi gốc: {request.Prompt}. Hãy báo cáo kết quả.");

                        var reportResponse = await chatService.GetChatMessageContentAsync(reportHistory);
                        finalResponseText = reportResponse.Content;
                    }

                    // Nếu thành công thì thoát vòng lặp ngay
                    goto SaveAndReturn;
                }
                catch (Exception ex)
                {
                    // BƯỚC C: GẶP LỖI -> KÍCH HOẠT CƠ CHẾ TỰ SỬA (FIXING MODE)
                    lastError = ex.Message;
                    Console.WriteLine($"[ATTEMPT {currentAttempt} FAILED]: {lastError}");

                    // 1. Ghi lại lỗi vào DB để học lâu dài
                    await _historyService.SaveLearningErrorAsync(request.Prompt, currentThoughtData.Sql, lastError);

                    // 2. Nếu đã hết lượt thử -> Báo lỗi cho User
                    if (currentAttempt >= maxRetries)
                    {
                        finalResponseText = $"⚠️ **Thất bại sau {maxRetries} lần thử:** Tôi gặp khó khăn kỹ thuật với câu hỏi này.\n- **Lỗi cuối cùng:** {lastError}";
                        break;
                    }

                    // 3. CHƯA HẾT LƯỢT -> NẠP LỖI VÀO CONTEXT ĐỂ AI SỬA
                    // Mẹo: Đóng vai Assistant (SQL cũ) và User (Thông báo lỗi)
                    sqlHistory.AddAssistantMessage(aiResponse.Content); // Nhét lại câu JSON cũ vào mồm nó

                    string fixPrompt = $@"
⚠️ HỆ THỐNG BÁO LỖI SQL:
{lastError}

YÊU CẦU SỬA LỖI:
1. Đọc kỹ Error Message trên (VD: Invalid column name nghĩa là cột không tồn tại trong bảng).
2. Xem lại Schema (Phần II) để tìm tên cột đúng.
3. Trả về JSON mới chứa câu SQL đã sửa.
";
                    sqlHistory.AddUserMessage(fixPrompt);

                    // Quay lại đầu vòng lặp while -> Gọi AI lại với Context mới
                }
            }

        // ==================================================================================
        // PHASE 5: CLOSING
        // ==================================================================================
        SaveAndReturn:;

            if (!string.IsNullOrEmpty(finalResponseText))
            {
                await _historyService.SaveLogAsync(sessionId, "user", request.Prompt);
                await _historyService.SaveLogAsync(sessionId, "assistant", finalResponseText);
            }

            return Ok(new { answer = finalResponseText });
        }
        // ---------------------------------------------------------
        // HELPER METHODS (Giữ nguyên như cũ)
        // ---------------------------------------------------------
        public class AiThoughtProcess
        {
            public string Thought { get; set; } // Suy nghĩ của AI
            public string Sql { get; set; }     // Câu lệnh SQL
        }
        private AiThoughtProcess ParseAiJson(string aiContent)
        {
            try
            {
                var cleanJson = aiContent.Replace("```json", "").Replace("```", "").Trim();
                return JsonConvert.DeserializeObject<AiThoughtProcess>(cleanJson);
            }
            catch
            {
                // Fallback an toàn nếu AI trả về lỗi định dạng
                return new AiThoughtProcess
                {
                    Thought = aiContent, // Coi toàn bộ nội dung là câu trả lời text
                    Sql = "NONE"
                };
            }
        }

        private string GetDynamicSchema(string userPrompt)
        {
            var normalizedPrompt = userPrompt.ToLower();
            var schemaParts = new List<string>();

            // =========================================================================================
            // 1. CORE SYSTEM (Giữ nguyên)
            // =========================================================================================
            string coreSchema = @"
--- [CORE INFRASTRUCTURE] ---
1. Branches(branch_id, name, address) 
   -- [DESC]: Chi nhánh cửa hàng (VD: Thuận An, Thủ Dầu Một).
   -- [PATH]: Là điểm bắt đầu của mọi bộ lọc địa điểm.

2. Warehouses(warehouse_id, branch_id, name) 
   -- [DESC]: Kho chứa hàng thuộc về một chi nhánh.
   -- [PATH]: Branches -> Warehouses -> Inventory.

3. ProductCategories(category_id, name) 
   -- [DESC]: Nhóm hàng (Bia, Sữa, Rau củ...).
   -- [PATH]: ProductCategories -> Products.
";
            schemaParts.Add(coreSchema);

            // =========================================================================================
            // 2. INVENTORY DOMAIN (Bổ sung nhắc nhở về tên cột)
            // =========================================================================================
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"tồn|kho|lô|hết hạn|nhập|hàng|stock|inventory|còn|bao nhiêu|hiện tại|sắp hết|expiry|sản phẩm"))
            {
                schemaParts.Add(@"
--- [INVENTORY DOMAIN] ---
* [GOAL]: Truy vấn số lượng hàng ĐANG CÓ thực tế, Hạn sử dụng và Lịch sử biến động.

1. Products(product_id, name, category_id, supplier_id, barcode, price, unit, min_stock) 
   -- [DESC]: Thông tin chung của sản phẩm.
   -- [IMPORTANT]: Cột tên sản phẩm là 'name'. Tuyệt đối không bịa ra 'ProductName'.
   
2. ProductLots(lot_id, product_id, expiry_date, cost_price, batch_code) 
   -- [DESC]: Quản lý Lô hàng nhập vào. 'cost_price' là Giá Vốn.
   -- [LOGIC]: Tìm hàng hết hạn dùng: WHERE expiry_date < GETDATE().
   -- [PATH]: Products -> ProductLots (1-n).

3. Inventory(inventory_id, warehouse_id, lot_id, quantity) 
   -- [DESC]: Số lượng tồn kho chi tiết theo từng Lô tại từng Kho.
   -- [PATH QUAN TRỌNG]: Inventory kết nối với Products THÔNG QUA ProductLots.
      (Inventory.lot_id -> ProductLots.lot_id -> Products.product_id).
   -- [LOGIC]: Tồn kho = SUM(quantity). Luôn lọc quantity > 0.
4. StockMovements(movement_id, product_id, movement_type, quantity, reason, movement_date)
   -- [DESC]: Lịch sử xuất/nhập/điều chuyển kho.
   -- [VALUES]: Cột 'movement_type' CHỈ NHẬN các giá trị sau:
      + 'Import'     : Nhập hàng mới.
      + 'Return'     : Trả hàng (về nhà cung cấp hoặc khách trả lại).
      + 'Sale'       : Xuất bán (khi tạo đơn hàng).
      + 'Adjustment' : Kiểm kê / Cân bằng kho (khi kho thực tế khác hệ thống).
      + 'Internal'   : Điều chuyển nội bộ (từ kho này sang kho khác).
");
            }

            // =========================================================================================
            // 3. SALES DOMAIN (Giữ nguyên)
            // =========================================================================================
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"bán|doanh thu|đơn|tiền|lợi nhuận|top|chạy nhất|mua|sales|order|bill|revenue"))
            {
                schemaParts.Add(@"
--- [SALES DOMAIN] ---
* [GOAL]: Truy vấn lịch sử bán hàng và dòng tiền thu về.

1. Orders(order_id, branch_id, order_date, status, total_amount, customer_id)
   -- [DESC]: Đơn hàng tổng. 
   -- [LOGIC]: Chỉ tính đơn thành công (status = 'Completed').
   -- [PATH]: Branches -> Orders.

2. OrderDetails(order_detail_id, order_id, product_id, quantity, unit_price, discount)
   -- [DESC]: Chi tiết từng món trong đơn hàng.
   -- [LOGIC]: Tìm sản phẩm bán chạy = SUM(quantity) GROUP BY product_id.
   -- [PATH]: Orders -> OrderDetails -> Products.

3. Quotations(quotation_id, total_amount, status, expiry_date) 
   -- [DESC]: Báo giá gửi khách (Chưa phải doanh thu).
");
            }

            // =========================================================================================
            // 4. CUSTOMER DOMAIN (FIX LỖI LEVEL 4: QUÊN TÊN CỘT)
            // =========================================================================================
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"khách|vip|điểm|thành viên|ai mua|customer|loyalty|người dùng|người mua"))
            {
                schemaParts.Add(@"
--- [CUSTOMER DOMAIN] ---
* [SECURITY]: TUYỆT ĐỐI KHÔNG dùng bảng Customers gốc. Dữ liệu phải được ẩn danh.

1. v_AI_Customer_Analytics(customer_id, customer_type, points_balance, masked_name)
   -- [DESC]: View tổng hợp thông tin khách hàng.
   -- [WARNING]: Cột tên khách hàng là 'masked_name'. KHÔNG ĐƯỢC DÙNG 'CustomerName' hay 'Name'.
   -- [PATH]: v_AI_Customer_Analytics -> Orders (qua customer_id).

2. LoyaltyPrograms(loyalty_id, customer_id, points_earned, points_redeemed)
   -- [DESC]: Lịch sử tích điểm và đổi điểm.
");
            }

            // =========================================================================================
            // 5. FINANCE & PARTNERS (Giữ nguyên)
            // =========================================================================================
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"nợ|chi phí|trả|lỗ|lãi|nhà cung cấp|expense|debt|supplier|nhập hàng|đối tác"))
            {
                schemaParts.Add(@"
--- [FINANCE DOMAIN] ---
* [GOAL]: Quản lý chi phí vận hành và công nợ đối tác.

1. Suppliers(supplier_id, name, phone, email)
   -- [DESC]: Nhà cung cấp hàng hóa.

2. PurchaseOrders(purchase_order_id, supplier_id, total_amount, status)
   -- [DESC]: Đơn nhập hàng từ nhà cung cấp (Đầu vào).
   -- [PATH]: Suppliers -> PurchaseOrders.

3. SupplierDebts(debt_id, supplier_id, total_amount, unpaid_amount, status)
   -- [DESC]: Công nợ phải trả. 'unpaid_amount' là số tiền còn nợ.

4. Expenses(expense_id, branch_id, amount, expense_date, note, category_id)
   -- [DESC]: Chi phí nội bộ (Điện, Nước, Lương...). Khác với tiền nhập hàng.
   -- [PATH]: Branches -> Expenses.
");
            }

            // =========================================================================================
            // 6. AI & ANALYTICS (FIX LỖI LEVEL 5: TỰ TÍNH TAY)
            // =========================================================================================
            // Bổ sung keyword: "ngày tới", "days", "nguy cơ", "cháy hàng"
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"dự báo|tương lai|cảnh báo|gợi ý|thông minh|forecast|predict|warn|risk|xu hướng|ngày tới|days|nguy cơ|cháy hàng"))
            {
                schemaParts.Add(@"
--- [AI INSIGHTS DOMAIN - ƯU TIÊN CAO NHẤT] ---
* [GOAL]: Dữ liệu phân tích nâng cao từ Machine Learning.
* [MANDATORY RULE]: Nếu câu hỏi chứa 'ngày tới', 'nguy cơ', 'cháy hàng', BẮT BUỘC phải dùng bảng bên dưới. KHÔNG được tự tính toán (min_stock - quantity).

1. AI_SalesForecast(product_id, branch_id, forecast_date, predicted_qty, confidence_score)
   -- [DESC]: Dự đoán số lượng bán trong tương lai (Next 7-30 days).

2. AI_InventoryWarning(product_id, days_to_exhaust, warning_type, risk_reason)
   -- [DESC]: Cảnh báo rủi ro kho. 'days_to_exhaust': Số ngày còn lại trước khi hết hàng.
   -- [LOGIC]: 'Hết hàng trong X ngày tới' nghĩa là: WHERE days_to_exhaust <= X.
   -- [PATH]: AI_InventoryWarning -> Products (qua product_id).

3. AI_ReplenishmentAdvice(product_id, branch_id, suggested_qty, priority_level)
   -- [DESC]: Gợi ý nhập hàng. Priority: 'High', 'Medium', 'Low'.
");
            }

            // Fallback
            if (schemaParts.Count == 1)
            {
                schemaParts.Add(@"
--- [FALLBACK CONTEXT] ---
1. Products(product_id, name, price)
2. Inventory(quantity, warehouse_id)
3. Orders(total_amount, status)
");
            }

            return string.Join("\n", schemaParts);
        }
    }

    public class ChatRequest
    {
        public required string Prompt { get; set; }
        public string? UserName { get; set; }
    }
}