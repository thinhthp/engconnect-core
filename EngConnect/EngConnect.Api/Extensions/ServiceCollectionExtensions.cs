using EngConnect.Entities.Entities;
using EngConnect.Repositories.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
    }
}
