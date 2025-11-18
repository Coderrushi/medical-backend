using medical_record_system_backend.Data;
using medical_record_system_backend.Repositories;
using medical_record_system_backend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using medical_record_system_backend.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
var configuration = builder.Configuration;

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<FileUploadOperationFilter>();
});

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories & services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileService, FileService>();

// Controllers
builder.Services.AddControllers();

// Cookie auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MedicalAuth";
        options.Cookie.HttpOnly = true;
        options.LoginPath = "/api/auth/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure wwwroot/uploads exists
var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true
});

app.UseStaticFiles(); // serve /wwwroot
app.UseRouting();
app.UseCors("DevCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();


//namespace medical_record_system_backend
//{






//    //public class Program
//    //{
//    //    public static void Main(string[] args)
//    //    {
//    //        var builder = WebApplication.CreateBuilder(args);

//    //        // Add services to the container.

//    //        builder.Services.AddControllers();
//    //        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//    //        builder.Services.AddEndpointsApiExplorer();
//    //        builder.Services.AddSwaggerGen();

//    //        var app = builder.Build();

//    //        // Configure the HTTP request pipeline.
//    //        if (app.Environment.IsDevelopment())
//    //        {
//    //            app.UseSwagger();
//    //            app.UseSwaggerUI();
//    //        }

//    //        app.UseHttpsRedirection();

//    //        app.UseAuthorization();


//    //        app.MapControllers();

//    //        app.Run();
//    //    }
//    //}
//}
