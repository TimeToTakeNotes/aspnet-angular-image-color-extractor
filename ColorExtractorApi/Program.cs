using Microsoft.EntityFrameworkCore;
using ColorExtractorApi.Data;
using ColorExtractorApi.Services;
using ColorExtractorApi.Repository;

var MyAllowFrontend = "myAllowFrontend";
var builder = WebApplication.CreateBuilder(args);

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

// Dynamic connection string:
var serverName = Environment.GetEnvironmentVariable("SERVER_NAME") ?? "localhost";
var connectionString = $"Server={serverName};Database=ColorExtractorDb;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<ImageDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IImageProcessor, ImageProcessor>();
builder.Services.AddScoped<IImageSaver, ImageSaver>();
builder.Services.AddScoped<ImageProcessingService>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();


var app = builder.Build();

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
