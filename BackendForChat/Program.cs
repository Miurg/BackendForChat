using BackendForChat.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Middleware;
using BackendForChat.Application.Services;
using BackendForChat.Models.Entities;
using System.Reflection;
using BackendForChat.Application.Interfaces;

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

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
builder.WebHost.UseKestrel(options =>
{
    options.Listen(IPAddress.Any, 5001); // HTTP
    options.Listen(IPAddress.Any, 7168, listenOptions =>
    {
        listenOptions.UseHttps(builder.Configuration["HTTPS:CertPath"], builder.Configuration["HTTPS:CertName"]);
    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

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
