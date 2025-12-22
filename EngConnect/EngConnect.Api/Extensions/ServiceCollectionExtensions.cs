using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using EngConnect.Services.Caching.Upstash;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text;

namespace EngConnect.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<EngConnectContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection jwtSettings = configuration.GetSection("JwtSettings");
            string? secretKey = jwtSettings.GetValue<string>("Secret");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // True for production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };

                //Configure SignalR authentication
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        var accessToken = ctx.Request.Query["access_token"];
                        var path = ctx.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                            ctx.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["ClientId"]!;
                options.ClientSecret = googleAuthNSection["ClientSecret"]!;
                options.CallbackPath = "/signin-google";
                options.SignInScheme = IdentityConstants.ExternalScheme;
            })
            ;

            return services;
        }

        public static IServiceCollection AddUpstash(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<UpstashRedisOptions>(
                configuration.GetSection("UpstashRedis"));

            services.AddHttpClient("UpstashClient", (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<UpstashRedisOptions>>().Value;

                if (string.IsNullOrWhiteSpace(options.RestUrl))
                {
                    throw new InvalidOperationException("UpstashRedis:RestUrl is not configured.");
                }

                if (string.IsNullOrWhiteSpace(options.RestToken))
                {
                    throw new InvalidOperationException("UpstashRedis:RestToken is not configured.");
                }

                client.BaseAddress = new Uri(options.RestUrl.TrimEnd('/'));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.RestToken);
            });

            return services;
        }
    }
}
