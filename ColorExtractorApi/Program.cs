using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ColorExtractorApi.Data;
using ColorExtractorApi.Helpers;
using ColorExtractorApi.Repository;
using ColorExtractorApi.Services;

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
            .AllowAnyMethod());
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

// Register DbContext with the final connection str
builder.Services.AddDbContext<ColorExtractorContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtUtils>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ImageProcessor>();
builder.Services.AddScoped<ImageSaver>();


var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ColorExtractorContext>();
    Console.WriteLine("Connected to DB: " + db.Database.GetDbConnection().ConnectionString);
});

app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowFrontend);
app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.Run();
