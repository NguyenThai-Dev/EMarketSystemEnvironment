using System.Threading.RateLimiting;
using AIAssistantService;
using AIAssistantService.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Http.Resilience;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 2. Cấu hình Key Rotation - Đăng ký Singleton cho Gemini
builder.Services.AddSingleton<IApiKeyProvider, GeminiApiKeyProvider>();

// 3. Cấu hình HttpClient cho EMARKET
// Cho phép hệ thống truy cập vào HttpContext hiện tại
builder.Services.AddHttpContextAccessor();

// Đăng ký TokenForwardingHandler
builder.Services.AddTransient<TokenForwardingHandler>();

var domainEMarket = builder.Configuration["DomainSettings:DomainEMarket"] ?? "";
var domainGemini = builder.Configuration["DomainSettings:DomainGemini"]
    ?? builder.Configuration["DomainSettings:DomainGoogle"]
    ?? "https://generativelanguage.googleapis.com/v1beta/openai/";

var modelGemini = builder.Configuration["GeminiSettings:ModelId"]
    ?? "gemini-2.5-flash";

// 3. Cấu hình HttpClient cho EMARKET
builder.Services.AddHttpClient("EMarketClient", client =>
{
    client.BaseAddress = new Uri(domainEMarket);
    // client.Timeout được xử lý bởi Polly Resilience Pipeline
    client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
})
// Ép EMarketClient phải đi qua trạm thu phí để lấy Token
.AddHttpMessageHandler<TokenForwardingHandler>()
.AddStandardResilienceHandler(options =>
{
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);

    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);

    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(10);
});

// 3.5 Cấu hình HttpClient cho GEMINI (Chuẩn OpenAI tương thích của Google)
builder.Services.AddHttpClient("GeminiClient", client =>
{
    client.BaseAddress = new Uri(domainGemini);
})
.AddStandardResilienceHandler(options =>
{
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(25);

    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);

    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
    options.Retry.MaxRetryAttempts = 3;
});

// 4. History Services
var connectionString = builder.Configuration["DatabaseSettings:ConnectionString"] ?? "";
builder.Services.AddScoped<IAiHistoryService>(sp => new AiHistoryService(connectionString));

// Đăng ký EMarketApiPlugin
builder.Services.AddScoped<EMarketApiPlugin>();

// 5. Cấu hình Semantic Kernel (Transient để đổi Key theo từng Request)
builder.Services.AddTransient<Kernel>(sp =>
{
    var keyProvider = sp.GetRequiredService<IApiKeyProvider>();
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var geminiClient = httpClientFactory.CreateClient("GeminiClient");
    var kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0001
    kernelBuilder.AddOpenAIChatCompletion(
        modelId: modelGemini,
        apiKey: keyProvider.GetApiKey(), // Bốc Gemini key xoay tua ở đây
        endpoint: new Uri(domainGemini),
        httpClient: geminiClient
    );
#pragma warning restore SKEXP0001

    kernelBuilder.Plugins.AddFromObject(new TimePlugin(), "Time");
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<EMarketApiPlugin>(), "EMarketAPI");

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
