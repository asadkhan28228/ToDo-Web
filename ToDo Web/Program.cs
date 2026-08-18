using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using ToDo.BLL.Interface;
using ToDo.BLL.Services;
using ToDo.DAL.Context;
using ToDo.DAL.IRepository;
using ToDo.DAL.Repository;
using ToDo_Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// Controllers
// ==============================
builder.Services.AddControllers();


// ==============================
// OpenAPI / Scalar
// ==============================
builder.Services.AddOpenApi();


// ==============================
// SQL Server
// ==============================
builder.Services.AddDbContext<ToDoContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString("DefaultConnection")
        );
    });


// ==============================
// SQLite
// ==============================
var sqlitePath = Path.Combine(
    builder.Environment.ContentRootPath,
    "ToDo_local.db"
);

builder.Services.AddDbContext<SqliteToDoContext>(
    options =>
    {
        options.UseSqlite(
            $"Data Source={sqlitePath};Default Timeout=30;"
        );
    });


// ==============================
// JWT Authentication
// ==============================
builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme
    )
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    ),

                NameClaimType =
                    ClaimTypes.Name,

                RoleClaimType =
                    ClaimTypes.Role
            };
    });


// ==============================
// Repositories
// ==============================
builder.Services.AddScoped<
    IAuthRepository,
    AuthRepository>();

builder.Services.AddScoped<
    IProjectRepository,
    ProjectRepository>();


// ==============================
// Services
// ==============================
builder.Services.AddScoped<
    IJwtService,
    JwtServices>();

builder.Services.AddScoped<
    IAuthService,
    AuthServices>();

builder.Services.AddScoped<
    IProjectService,
    ProjectService>();


// ==============================
// Background SQLite → SQL Sync
// ==============================
builder.Services.AddHostedService<
    TaskSyncService>();


var app = builder.Build();


// ==============================
// Create SQLite Database
// ==============================
using (var scope = app.Services.CreateScope())
{
    var sqliteContext =
        scope.ServiceProvider
            .GetRequiredService<SqliteToDoContext>();

    sqliteContext.Database.EnsureCreated();

    Console.WriteLine(
        "SQLite Database Path: " +
        sqliteContext.Database
            .GetDbConnection()
            .DataSource
    );
}


// ==============================
// Development
// ==============================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}


app.UseHttpsRedirection();


// IMPORTANT
app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();