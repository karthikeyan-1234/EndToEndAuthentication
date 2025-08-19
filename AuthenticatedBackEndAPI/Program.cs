using AuthenticatedBackEndAPI.Contexts;

using Microsoft.AspNetCore.Authentication.JwtBearer;


using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<EmployeeContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://192.168.1.150:8091/realms/master"; // Replace with your auth server URL
        options.Audience = "api-app"; // Replace with your API audience,
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "http://192.168.1.150:8091/realms/master",
            ValidAudience = "api-app",
            RoleClaimType = "roles" // <- or "realm_access.roles" if nested
        };

        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(System.Security.Authentication.AuthenticationException))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Invalid token\"}");
                }
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal!.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    var roleClaims = context.Principal.FindAll("roles");
                    foreach (var roleClaim in roleClaims)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                    }
                }
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                // Custom logic for handling challenges
                if (context.AuthenticateFailure != null)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Authentication failed\"}");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

AppContext.SetSwitch("Microsoft.IdentityModel.Logging.ShowPII", true);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authenticated BackEnd API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
