using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EngConnect.Services.Caching.RateLimiting
{
    public sealed class RedisRateLimitStore : IRateLimitStore
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RedisRateLimitStore> _logger;

        public RedisRateLimitStore(
            IHttpClientFactory httpClientFactory,
            ILogger<RedisRateLimitStore> logger)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            _httpClient = httpClientFactory.CreateClient("UpstashClient");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> IncrementAsync(string key, TimeSpan window, CancellationToken cancellationToken = default)
        {
            var newValue = await ExecuteCommandAsync<long>(new[] { "INCR", key }, cancellationToken)
                .ConfigureAwait(false);

            if (newValue == 1)
            {
                var ttlSeconds = (int)window.TotalSeconds;
                if (ttlSeconds > 0)
                {
                    _ = ExecuteCommandAsync<object>(
                            new[] { "EXPIRE", key, ttlSeconds.ToString() },
                            cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            return newValue;
        }

        private async Task<T> ExecuteCommandAsync<T>(IReadOnlyList<string> command, CancellationToken cancellationToken)
        {
            var envelope = new[] { command };
            var payload = JsonSerializer.Serialize(envelope);
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("/pipeline", content, cancellationToken)
                .ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Upstash returned {StatusCode}: {ReasonPhrase}. Body: {Body}",
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    responseBody);

                throw new InvalidOperationException(
                    $"Upstash returned {(int)response.StatusCode}: {response.ReasonPhrase}. Body: {responseBody}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
            {
                throw new InvalidOperationException("Invalid Upstash pipeline response: root is not array or empty.");
            }

            var first = root[0];
            if (first.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException("Invalid Upstash pipeline response: first element is not object.");
            }

            if (first.TryGetProperty("error", out var errorElement) &&
                errorElement.ValueKind == JsonValueKind.String)
            {
                var errorMessage = errorElement.GetString();
                throw new InvalidOperationException($"Upstash command error: {errorMessage}");
            }

            if (first.TryGetProperty("result", out var resultElement))
            {
                if (typeof(T) == typeof(long) && resultElement.ValueKind == JsonValueKind.Number)
                {
                    var value = resultElement.GetInt64();
                    return (T)(object)value;
                }

                return default!;
            }

            throw new InvalidOperationException("Invalid Upstash pipeline response: missing 'result' or 'error'.");
        }
    }
}