using BackendForChat.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Newtonsoft.Json.Serialization;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Middleware;
using BackendForChat.Application.Services;
using BackendForChat.Models.Entities;

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
builder.Services.AddSingleton<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();  
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
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

}
app.UseMiddleware<ValidateUserMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapHub<MessageHub>("/messageHub");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
