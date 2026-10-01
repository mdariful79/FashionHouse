using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FashionHouse.Web.Services;

public class ReCaptchaOptions
{
    public string SiteKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public double MinScore { get; set; } = 0.5;
}

public interface IReCaptchaService
{
    Task<bool> VerifyAsync(string? token, string expectedAction, CancellationToken ct = default);
}

public class ReCaptchaService : IReCaptchaService
{
    private readonly HttpClient _http;
    private readonly ReCaptchaOptions _options;
    private readonly ILogger<ReCaptchaService> _logger;

    public ReCaptchaService(
        HttpClient http,
        IOptions<ReCaptchaOptions> options,
        ILogger<ReCaptchaService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> VerifyAsync(string? token, string expectedAction, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("reCAPTCHA: token was empty (JS did not set RecaptchaToken).");
            return false;
        }

        var response = await _http.PostAsync(
            "https://www.google.com/recaptcha/api/siteverify",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["secret"] = _options.SecretKey,
                ["response"] = token
            }), ct);

        var json = await response.Content.ReadAsStringAsync(ct);
        _logger.LogInformation("reCAPTCHA response: {Json}", json);

        if (!response.IsSuccessStatusCode) return false;

        var result = JsonSerializer.Deserialize<ReCaptchaResponse>(json);

        return result is { Success: true }
               && string.Equals(result.Action, expectedAction, StringComparison.OrdinalIgnoreCase)
               && result.Score >= _options.MinScore;
    }

    private class ReCaptchaResponse
    {
        [JsonPropertyName("success")] public bool Success { get; set; }
        [JsonPropertyName("score")] public double Score { get; set; }
        [JsonPropertyName("action")] public string? Action { get; set; }
        [JsonPropertyName("hostname")] public string? Hostname { get; set; }
        [JsonPropertyName("error-codes")] public string[]? ErrorCodes { get; set; }
    }
}