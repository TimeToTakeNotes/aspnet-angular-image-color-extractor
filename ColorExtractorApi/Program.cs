using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

using ColorExtractorApi.Data;
using ColorExtractorApi.Repository;
using ColorExtractorApi.Services;
using ColorExtractorApi.Services.Helpers;

var MyAllowFrontend = "myAllowFrontend";
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); // Load env vars

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowFrontend,
        policy => policy
            .WithOrigins("http://localhost:4200") // frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Get env var SERVER_NAME or fallback to null if not set
var serverName = Environment.GetEnvironmentVariable("SERVER_NAME");

// Get connection str from appsettings.json (DefaultConnection)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// If SERVER_NAME env var is set, override the server part
if (!string.IsNullOrEmpty(serverName))
{
    var builderConn = new SqlConnectionStringBuilder(connectionString)
    {
        DataSource = serverName
    };
    connectionString = builderConn.ConnectionString;
}

// Load JWT key from env or config
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
          ?? builder.Configuration["JwtSettings:Key"];

if (string.IsNullOrEmpty(jwtKey))
    throw new Exception("JWT key missing in environment variables or configuration.");


// Register DbContext with the final connection str
builder.Services.AddDbContext<ColorExtractorContext>(options =>
    options.UseSqlServer(connectionString)
);


// Cotnrollers:
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// JWT Authentication:
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// Repositories:
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

// Services:
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IImageService, ImageService>();

//Service Helpers:
builder.Services.AddScoped(sp => new JwtUtils(jwtKey)); // Pass key to JwtUtils via DI
builder.Services.AddScoped<ImageProcessor>();
builder.Services.AddScoped<ImageSaver>();


var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ColorExtractorContext>();
    Console.WriteLine("Connected to DB: " + db.Database.GetDbConnection().ConnectionString);
});

app.Use(async (context, next) =>
{
    if (context.Request.Cookies.TryGetValue("access_token", out var accessToken))
    {
        // Add Authorization header with Bearer scheme
        context.Request.Headers["Authorization"] = "Bearer " + accessToken;
    }
    await next();
});

app.UseStaticFiles(); // Serve wwwroot folder
app.UseRouting();
app.UseCors(MyAllowFrontend);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.Run();
