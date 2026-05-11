using System.Threading.RateLimiting;
using AIAssistantService;
using AIAssistantService.Plugins;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 2. Cấu hình Key Rotation (Xoay tua API) - Đăng ký Singleton là chính xác
builder.Services.AddSingleton<IApiKeyProvider, GroqApiKeyProvider>();
builder.Services.AddSingleton<IPromptService, PromptService>();

// 3. Cấu hình HttpClient cho EMARKET
// Cho phép hệ thống truy cập vào HttpContext hiện tại
builder.Services.AddHttpContextAccessor();

// Đăng ký TokenForwardingHandler
builder.Services.AddTransient<TokenForwardingHandler>();

// 3. Cấu hình HttpClient cho EMARKET
builder.Services.AddHttpClient("EMarketClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44338/");
    client.Timeout = TimeSpan.FromMinutes(10);
    client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
})
// Ép EMarketClient phải đi qua trạm thu phí để lấy Token
.AddHttpMessageHandler<TokenForwardingHandler>();

// 3.5 Cấu hình HttpClient cho GROQ (Chuẩn OpenAI tương thích)
builder.Services.AddHttpClient("GroqClient", client =>
{
    // Endpoint chuẩn của Groq cho các request tương thích OpenAI
    client.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
    client.Timeout = TimeSpan.FromMinutes(5);
});

// 4. History Services
var connectionString = builder.Configuration["DatabaseSettings:ConnectionString"] ?? "";
builder.Services.AddScoped<DatabasePlugin>(sp => new DatabasePlugin(connectionString));
builder.Services.AddScoped<IAiHistoryService>(sp => new AiHistoryService(connectionString));

// 5. Cấu hình Semantic Kernel (Transient để đổi Key theo từng Request)
builder.Services.AddTransient<Kernel>(sp =>
{
    var keyProvider = sp.GetRequiredService<IApiKeyProvider>();
    var kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0001
    kernelBuilder.AddOpenAIChatCompletion(
        modelId: "llama-3.3-70b-versatile",
        apiKey: keyProvider.GetApiKey(), // Bốc key xoay tua ở đây
        endpoint: new Uri("https://api.groq.com/openai/v1")
    );
#pragma warning restore SKEXP0001

    kernelBuilder.Plugins.AddFromObject(new TimePlugin(), "Time");
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<DatabasePlugin>(), "EMarketDB");

    return kernelBuilder.Build();
});

builder.Services.AddRateLimiter(options =>
{
    // Cấu hình chặn toàn cục (Global)
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(20),
                QueueLimit = 0
            });
    });

    // Trả về lỗi 429 khi quá tải
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Middleware Stack
app.UseMiddleware<IpWhitelistMiddleware>();
app.UseCors("AllowAll");
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
