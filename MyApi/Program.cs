using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ThirdPartyOptions>().BindConfiguration("ThirdParty");

var app = builder.Build();

// Fail fast with a clear message if secrets were not injected
var apiKey = app.Configuration["ThirdParty:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException(
        "ThirdParty:ApiKey is missing. Start the app with: infisical run --env=dev -- dotnet run");
}

app.MapGet("/health", () => Results.Ok("ok"));

// DEMO ONLY: shows that secrets were loaded without exposing their values.
// Never return real secret values from an endpoint.
app.MapGet("/demo", (IConfiguration config, IOptions<ThirdPartyOptions> opts) => new
{
    ApiKeyLoaded = !string.IsNullOrEmpty(config["ThirdParty:ApiKey"]),
    ApiKeyLength = opts.Value.ApiKey.Length,
    DbConfigured = !string.IsNullOrEmpty(config["ConnectionStrings:Default"])
});

app.Run();

public class ThirdPartyOptions
{
    public string ApiKey { get; set; } = "";
}