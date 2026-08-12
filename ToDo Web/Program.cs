using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using ToDo.BLL.Interface;
using ToDo.BLL.Services;
using ToDo.DAL.Context;
using ToDo.DAL.IRepository;
using ToDo.DAL.Repository;
using ToDo_Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// registered Sql Connection string 
builder.Services.AddDbContext<ToDoContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Connection string for SQLite
var sqlitePath = Path.Combine(
    builder.Environment.ContentRootPath,
    "todo_local.db"
);

builder.Services.AddDbContext<SqliteToDoContext>(options =>
{
    options.UseSqlite(
        $"Data Source={sqlitePath};Default Timeout=30;"
    );
});
//This is JWT Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            NameClaimType = ClaimTypes.Name,

            RoleClaimType = ClaimTypes.Role
        };
    });


// Register the repositories
builder.Services.AddScoped<IAuthRepository,AuthRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// Register the services
builder.Services.AddScoped<IJwtService, JwtServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddHostedService<TaskSyncService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sqliteContext =scope.ServiceProvider.GetRequiredService<SqliteToDoContext>();

    sqliteContext.Database.EnsureCreated();

    Console.WriteLine("SQLite Database Path: " +sqliteContext.Database.GetDbConnection().DataSource);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();
