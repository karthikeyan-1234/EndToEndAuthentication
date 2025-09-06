using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// 🔑 Configure JWT Authentication with Keycloak claim mapping
// Add Authentication & Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://localhost:8443/realms/master";
        options.Audience = "api-app";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudiences = new[] { "api-app", "angular-app", "master-realm", "account" },
            ValidIssuer = "http://localhost:8443/realms/master",
            ValidAudience = "api-app",
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var azp = context.Principal?.FindFirst("azp")?.Value;
                if (azp != "api-app" && azp != "angular-app")
                {
                    context.Fail("Invalid authorized party");
                    return Task.CompletedTask;
                }

                var identity = context.Principal?.Identity as ClaimsIdentity;
                if (identity == null) return Task.CompletedTask;

                var realmAccess = context.Principal?.FindFirst("realm_access")?.Value;
                if (!string.IsNullOrEmpty(realmAccess))
                {
                    using var doc = JsonDocument.Parse(realmAccess);
                    if (doc.RootElement.TryGetProperty("roles", out var roles))
                    {
                        foreach (var role in roles.EnumerateArray())
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                        }
                    }
                }

                var resourceAccess = context.Principal?.FindFirst("resource_access")?.Value;
                if (!string.IsNullOrEmpty(resourceAccess))
                {
                    using var doc = JsonDocument.Parse(resourceAccess);
                    if (doc.RootElement.TryGetProperty("api-app", out var apiApp) &&
                        apiApp.TryGetProperty("roles", out var roles))
                    {
                        foreach (var role in roles.EnumerateArray())
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                        }
                    }
                }

                return Task.CompletedTask;
            },


            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                Console.WriteLine("OnChallenge: " + context.ErrorDescription);
                return Task.CompletedTask;
            }

        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

AppContext.SetSwitch("Microsoft.IdentityModel.Logging.ShowPII", true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");


app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        Console.WriteLine(">>> RAW AUTH HEADER:");
        Console.WriteLine(context.Request.Headers["Authorization"].ToString());
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();




app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

});

app.MapControllers();

app.Run();
