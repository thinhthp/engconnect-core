using EngConnect.Api.Extensions;
using EngConnect.Api.Hubs;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using EngConnect.Services.Caching.TutorProfile;
using EngConnect.Services.Integrations.PayOS;
using EngConnect.Services.Services.AI;
using EngConnect.Services.Services.TutorSchedules;
using EngConnect.Services.Services.UserContext;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Net.payOS;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// PayOS configuration
builder.Services.Configure<PayOSOptions>(builder.Configuration.GetSection("PayOS"));
builder.Services.AddSingleton(sp =>
{
    var opts = sp.GetRequiredService<IOptions<PayOSOptions>>().Value;
    return new PayOS(opts.ClientId, opts.ApiKey, opts.ChecksumKey);
});
builder.Services.AddTransient<IPayOSClient, PayOSAdapter>();

// Redis configuration
builder.Services.AddSingleton<ITutorProfileCache, TutorProfileCache>();
builder.Services.AddHttpClient(nameof(TutorProfileCache));

builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor(); // To access HttpContext for user info
builder.Services.AddHttpClient<IAIService, AIService>();
builder.Services.Scan(scan =>
    scan.FromAssemblies(
        Assembly.GetExecutingAssembly(), // Main project assembly
        Assembly.GetAssembly(typeof(UserContextService))!, // Services project
        Assembly.GetAssembly(typeof(UnitOfWork))! // Data project
    )
    .AddClasses(classes => classes.Where(t =>
    (t.Name.EndsWith("Service") || t.Name.EndsWith("Repository")) &&
    !typeof(IHostedService).IsAssignableFrom(t) &&
    !typeof(IAIService).IsAssignableFrom(t))) // exclude services
    .AsImplementedInterfaces()
    .WithScopedLifetime());
builder.Services.AddHostedService<WeeklyScheduleGenerationHostedService>();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});

// Configure DbContext.
builder.Services.AddDbContext<EngConnectContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentityConfiguration();

// Configure Authentication (JWT)
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

// Add CORS services.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Forwarded headers for Render/proxies
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Migrate database
await app.ApplyMigrationsAsync();

// Apply Cors
app.UseCors("AllowAll");

// Seed roles
await app.SeedRolesAsync();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
