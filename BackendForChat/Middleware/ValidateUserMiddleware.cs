using BackendForChat.Models.DatabaseContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BackendForChat.Middleware
{
    public class ValidateUserMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidateUserMiddleware> _logger;

        public ValidateUserMiddleware(RequestDelegate next, ILogger<ValidateUserMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
            var path = context.Request.Path.Value;
            if (path.Contains("api/auth")) 
            {
                await _next(context);
                return;
            }

            var authResult = await context.AuthenticateAsync();
            if (authResult.Succeeded && authResult.Principal != null)
            {
                context.User = authResult.Principal;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User not authenticated");
                return;
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid user ID format");
                return;
            }

            var userExists = await dbContext.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("User does not exist or has been deleted");
                return;
            }

            await _next(context);
        }
    }
}
