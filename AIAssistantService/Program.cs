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

// 2. Cấu hình Key Rotation - Đăng ký Singleton
builder.Services.AddSingleton<IApiKeyProvider, GroqApiKeyProvider>();

// 3. Cấu hình HttpClient cho EMARKET
// Cho phép hệ thống truy cập vào HttpContext hiện tại
builder.Services.AddHttpContextAccessor();

// Đăng ký TokenForwardingHandler
builder.Services.AddTransient<TokenForwardingHandler>();

var domainEMarket = builder.Configuration["DomainSettings:DomainEMarket"] ?? "";
var domainGroq = builder.Configuration["DomainSettings:DomainGroq"] ?? "";
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
    options.Retry.Delay = TimeSpan.FromSeconds(2);
});

// 3.5 Cấu hình HttpClient cho GROQ (Chuẩn OpenAI tương thích)
builder.Services.AddHttpClient("GroqClient", client =>
{
    // Endpoint chuẩn của Groq cho các request tương thích OpenAI
    client.BaseAddress = new Uri(domainGroq);
    // client.Timeout được xử lý bởi Polly Resilience Pipeline
})
.AddStandardResilienceHandler(options =>
{
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(20);

    // QUAN TRỌNG: Phải ít nhất 40s (20s * 2). 
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);

    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
    options.Retry.MaxRetryAttempts = 2;
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
    var kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0001
    kernelBuilder.AddOpenAIChatCompletion(
        //modelId: "llama-3.3-70b-versatile",
        modelId: "meta-llama/llama-4-scout-17b-16e-instruct",
        apiKey: keyProvider.GetApiKey(), // Bốc key xoay tua ở đây
        endpoint: new Uri(domainGroq)
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
