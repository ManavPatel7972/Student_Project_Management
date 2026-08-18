using Backend.Data;
using Backend.Helpers;
using Backend.Mapping;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

//1. Add DbContext
var connectionString = builder.Configuration.GetConnectionString("ManavConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

//2. Register Helpers & FluentValidation
builder.Services.AddScoped<JwtHelper>();

// Automatically execute those validators when requests come in.
builder.Services.AddFluentValidationAutoValidation();

// This is for All FluentValidation class
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


//3. Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();


#region Configure JWT Authentication (Swagger)
// 4. Configure JWT Auth
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "My_Super_key_1234567890";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SPMSApi",
        ValidAudience = jwtSettings["Audience"] ?? "SPMSClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };


    options.Events = new JwtBearerEvents
    {
        // 401 - Authentication failed
        OnChallenge = async context =>
        {
            // Suppress default challenge response
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>
            {
                Success = false,
                Errors = new List<string> { "Unauthorized access. Token is missing or invalid." }
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        },

        // 403 - Authorization failed
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>
            {
                Success = false,
                StatusCode = StatusCodes.Status403Forbidden,
                Errors = new List<string>
                {
                    "You do not have permission to access this resource."
                }
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    };
});

#endregion


//5. Add Authorization (Specify Roles)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("FacultyOrAdmin", policy => policy.RequireRole("Admin", "Faculty"));

    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

//6. Add swagger Authorization
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SPMS Web API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        // Swagger UI adds the "Bearer " prefix for an HTTP bearer scheme.
        Description = "Paste only the JWT token here (without the 'Bearer ' prefix).",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
    });
});

// 7. Configure CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseCors("AllowReactApp");

// app.UseHttpsRedirection();
app.UseStaticFiles(); // For uploaded profile photos

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
