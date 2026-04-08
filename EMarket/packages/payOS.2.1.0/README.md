# payOS .NET Library

[![NuGet](https://img.shields.io/nuget/v/payos.svg)](https://www.nuget.org/packages/payos/)

The payOS .NET library provides convenient access to the payOS Merchant API from applications written in C#.

To learn how to use payOS Merchant API, checkout our [API Reference](https://payos.vn/docs/api/) and [Documentation](https://payos.vn/docs/). We also have some examples in the [Examples](./examples/) directory.

## Requirements

This library currently support .NET Standard 2.0+, .NET Core 5+.

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package payOS
```

Or via Package Manager Console:

```bash
Install-Package payOS
```

> [!IMPORTANT]
> If update from v1, check [Migration guide](./MIGRATION.md) for detail migration.

## Usage

### Basic usage

First you need to initialize the client to interact with payOS Merchant API.

```csharp
using PayOS;

// Initialize the client
var client = new PayOSClient("your-client-id", "your-api-key", "your-checksum-key");

// Or initialize with options
var client = new PayOSClient(new PayOSOptions
{
    ClientId = "your-client-id",
    ApiKey = "your-api-key",
    ChecksumKey = "your-checksum-key"
});

// Or use default value in environment variables (recommended)
// Set PAYOS_CLIENT_ID, PAYOS_API_KEY, PAYOS_CHECKSUM_KEY, PAYOS_PARTNER_CODE
var client = new PayOSClient();
```

Then you can interact with payOS Merchant API, for example create a payment link using `PaymentRequests.CreateAsync()`.

```csharp
using PayOS.Models;

var paymentRequest = new CreatePaymentLinkRequest
{
    OrderCode = 123,
    Amount = 2000,
    Description = "payment",
    ReturnUrl = "https://your-url.com",
    CancelUrl = "https://your-url.com"
};

var paymentLink = await client.PaymentRequests.CreateAsync(paymentRequest);
```

### Webhook verification

You can register an endpoint to receive the payment webhook.

```csharp
var confirmResult = await client.Webhooks.ConfirmAsync("https://your-url.com/payos-webhook");
```

Then use `Webhooks.VerifyAsync()` to verify and receive webhook data.

```csharp
var webhookData = await client.Webhooks.VerifyAsync(new Webhook
{
    Code = "00",
    Description = "success",
    Success = true,
    Data = new WebhookData
    {
        OrderCode = 123,
        Amount = 3000,
        Description = "VQRIO123",
        AccountNumber = "12345678",
        Reference = "TF230204212323",
        TransactionDateTime = "2023-02-04 18:25:00",
        Currency = "VND",
        PaymentLinkId = "124c33293c43417ab7879e14c8d9eb18",
        Code = "00",
        Description2 = "Thành công",
        CounterAccountBankId = "",
        CounterAccountBankName = "",
        CounterAccountName = "",
        CounterAccountNumber = "",
        VirtualAccountName = "",
        VirtualAccountNumber = ""
    },
    Signature = "8d8640d802576397a1ce45ebda7f835055768ac7ad2e0bfb77f9b8f12cca4c7f"
});
```

For more information about webhooks, see [the API doc](https://client.vn/docs/api/#tag/payment-webhook/operation/payment-webhook).

### Handling errors

The library throws custom exceptions for different error scenarios:

```csharp
try
{
    var paymentLink = await client.PaymentRequests.CreateAsync(paymentData);
}
catch (ApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
catch (PayOSException ex)
{
    Console.WriteLine($"PayOS Error: {ex.Message}");
}
```

### Auto pagination

For endpoints that support pagination, you can use the built-in pagination:

```csharp
var payouts = await client.Payouts.ListAsync(limit: 20, offset: 0);
```

## Advanced usage

### Custom configuration

You can customize the PayOS client with various options:

```csharp
var client = new PayOSClient(new PayOSOptions
{
    ClientId = "your-client-id",
    ApiKey = "your-api-key",
    ChecksumKey = "your-checksum-key",
    PartnerCode = "your-partner-code", // Optional partner code
    BaseUrl = "https://api-merchant.client.vn", // Custom base URL
    TimeoutMs = 30000, // Request timeout in milliseconds (default: 60000)
    MaxRetries = 3, // Maximum retry attempts (default: 2)
    LogLevel = LogLevel.Information, // Log level
    Logger = logger, // Custom logger implementation
    HttpClient = httpClient, // Custom HttpClient instance
    DefaultHeaders = new Dictionary<string, string>
    {
        { "Custom-Header", "value" }
    }
});
```

### Custom HttpClient

You can provide a custom HttpClient instance:

```csharp
var httpClient = new HttpClient();
// Configure your HttpClient as needed

var client = new PayOSClient(new PayOSOptions
{
    ClientId = "your-client-id",
    ApiKey = "your-api-key",
    ChecksumKey = "your-checksum-key",
    HttpClient = httpClient
});
```

### Request-level options

You can override client-level settings for individual requests using CancellationToken:

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

var paymentLink = await client.PaymentRequests.CreateAsync(
    paymentData,
    cts.Token // Custom timeout for this request
);
```

### Logging and debugging

The SDK supports logging through Microsoft.Extensions.Logging:

```csharp
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<PayOS>();

var client = new PayOSClient(new PayOSOptions
{
    ClientId = "your-client-id",
    ApiKey = "your-api-key",
    ChecksumKey = "your-checksum-key",
    LogLevel = LogLevel.Debug, // Enable debug logging
    Logger = logger
});
```

### Direct API access

For advanced use cases, you can make direct API calls:

```csharp
// GET request
var response = await client.GetAsync<List<PaymentLink>>("/v2/payment-requests");

// POST request
var response = await client.PostAsync<CreatePaymentLinkResponse>("/v2/payment-requests", new
{
    orderCode = 123,
    amount = 2000,
    description = "payment",
    returnUrl = "https://your-url.com",
    cancelUrl = "https://your-url.com"
});
```

### Signature

The signature can be manually created by `PayOS.Crypto`:

```csharp
// for create-payment-link signature
var signature = await client.Crypto.CreateSignatureOfPaymentRequestAsync(data, client.ChecksumKey);

// for payment-requests and webhook signature
var signature = await client.Crypto.CreateSignatureFromObjectAsync(data, client.ChecksumKey);

// for payouts signature
var signature = await client.Crypto.CreateSignatureAsync(client.ChecksumKey, data);
```

## Contributing

Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.
