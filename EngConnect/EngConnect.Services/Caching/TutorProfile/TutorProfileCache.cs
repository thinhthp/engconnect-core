using EngConnect.Services.Caching.Upstash;
using EngConnect.Services.DTOs.TutorProfile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace EngConnect.Services.Caching.TutorProfile
{
    public class TutorProfileCache : ITutorProfileCache
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TutorProfileCache> _logger;
        private readonly int _defaultTtlSeconds;
        private const string KeyPrefix = "tutor-profile:";

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public TutorProfileCache(
            IHttpClientFactory httpClientFactory,
            IOptions<UpstashRedisOptions> options,
            ILogger<TutorProfileCache> logger)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            _httpClient = httpClientFactory.CreateClient("UpstashClient");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var opts = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _defaultTtlSeconds = opts.DefaultTtlSeconds > 0 ? opts.DefaultTtlSeconds : 1800;
        }

        public async Task<TutorProfileDTO?> GetAsync(string tutorId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tutorId))
            {
                return null;
            }

            var key = BuildKey(tutorId);
            try
            {
                var encodedKey = UrlEncoder.Default.Encode(key);
                var url = $"/get/{encodedKey}";

                using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("result", out var resultElement) &&
                    resultElement.ValueKind == JsonValueKind.String)
                {
                    var value = resultElement.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return JsonSerializer.Deserialize<TutorProfileDTO>(value, SerializerOptions);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get tutor profile cache for {TutorId}", tutorId);
            }

            return null;
        }

        public async Task SetAsync(TutorProfileDTO profile, CancellationToken cancellationToken = default)
        {
            if (profile == null || string.IsNullOrWhiteSpace(profile.TutorId))
            {
                return;
            }

            var key = BuildKey(profile.TutorId);
            var value = JsonSerializer.Serialize(profile, SerializerOptions);

            try
            {
                var encodedKey = UrlEncoder.Default.Encode(key);
                var encodedValue = UrlEncoder.Default.Encode(value);
                var url = $"/setex/{encodedKey}/{_defaultTtlSeconds}/{encodedValue}";

                using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to set tutor profile cache for {TutorId}", profile.TutorId);
            }
        }

        public async Task RemoveAsync(string tutorId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tutorId))
            {
                return;
            }

            var key = BuildKey(tutorId);

            try
            {
                var encodedKey = UrlEncoder.Default.Encode(key);
                var url = $"/del/{encodedKey}";

                using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove tutor profile cache for {TutorId}", tutorId);
            }
        }

        private static string BuildKey(string tutorId)
        {
            return $"{KeyPrefix}{tutorId}";
        }
    }
}