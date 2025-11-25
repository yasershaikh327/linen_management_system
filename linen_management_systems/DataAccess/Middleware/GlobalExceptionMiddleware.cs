using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace DataAccess.Middleware
{
  
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}");
                context.Response.ContentType = "application/json";

                if (ex is UnauthorizedAccessException)
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                else if (ex is KeyNotFoundException)
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                else
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new { success = false, message = "Oops! Something went wrong, please call support" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
