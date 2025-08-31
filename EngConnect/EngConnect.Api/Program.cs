using EngConnect.Api.Extensions;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using EngConnect.Services.Services.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor(); // To access HttpContext for user info
builder.Services.Scan(scan =>
    scan.FromAssemblies(
        Assembly.GetExecutingAssembly(), // Main project assembly
        Assembly.GetAssembly(typeof(UserContextService))!, // Services project
        Assembly.GetAssembly(typeof(UnitOfWork))! // Data project
    )
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

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

var app = builder.Build();

// Apply Cors
app.UseCors("AllowAll");

// Seed roles
await app.SeedRolesAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
