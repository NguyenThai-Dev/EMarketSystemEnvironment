using System.Threading.RateLimiting;
using AIAssistantService;
using AIAssistantService.Plugins;
using Microsoft.SemanticKernel;
// Thêm thư viện OpenAI connector
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 2. Cấu hình Key Rotation
builder.Services.AddSingleton<IApiKeyProvider, GroqApiKeyProvider>();

// 3. Cấu hình HttpClient cho EMARKET
builder.Services.AddHttpClient("EMarketClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44339/");
    client.Timeout = TimeSpan.FromMinutes(10);
    client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

// [MỚI] 3.5 Cấu hình HttpClient cho GROQ (Chuẩn OpenAI tương thích)
builder.Services.AddHttpClient("GroqClient", client =>
{
    // Endpoint chuẩn của Groq cho các request tương thích OpenAI
    client.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
    client.Timeout = TimeSpan.FromMinutes(5);
});

// 4. History Services
var connectionString = builder.Configuration["DatabaseSettings:ConnectionString"] ?? "";
builder.Services.AddScoped<IAiHistoryService>(sp => new AiHistoryService(connectionString));

// Đăng ký Plugin mới gọi API EMarket
builder.Services.AddScoped<EMarketApiPlugin>();

// 5. Cấu hình Semantic Kernel (Chuyển sang Groq)
builder.Services.AddTransient<Kernel>(sp =>
{
    var keyProvider = sp.GetRequiredService<IApiKeyProvider>();
    var apiKey = keyProvider.GetApiKey();

    // Lấy HttpClient của Groq
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var groqClient = httpClientFactory.CreateClient("GroqClient");

    var kernelBuilder = Kernel.CreateBuilder();

    // SỬ DỤNG OPENAI CONNECTOR ĐỂ GỌI GROQ
    kernelBuilder.AddOpenAIChatCompletion(
        modelId: "meta-llama/llama-4-scout-17b-16e-instruct", 
        apiKey: apiKey,
        httpClient: groqClient
    );

    // Đăng ký Plugins
    kernelBuilder.Plugins.AddFromObject(new TimePlugin(), "Time");
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<EMarketApiPlugin>(), "EMarketAPI");

    return kernelBuilder.Build();
});

// --- CÁC PHẦN CÒN LẠI GIỮ NGUYÊN ---
builder.Services.AddRateLimiter(options =>
{
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
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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