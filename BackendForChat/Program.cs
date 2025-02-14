using BackendForChat.Hubs;
using BackendForChat.Models;
using BackendForChat.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
        };
    });


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:db"]);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(new EncryptionService(builder.Configuration["JwtSettings:EncryptionKey"]));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();  
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();
builder.Services.AddSignalR();
builder.WebHost.UseKestrel(options =>
{
    options.Listen(IPAddress.Any, 5001); // HTTP
    options.Listen(IPAddress.Any, 7168, listenOptions =>
    {
        listenOptions.UseHttps("F:\\cert.pfx", "saymyname");
    });
});



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapHub<MessageHub>("/messageHub");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
