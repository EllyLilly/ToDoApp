using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace ToDoApp.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An unhandled exception occurred.");

                httpContext.Response.ContentType = "application/json";

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var responseObject = new
                {
                    StatusCode = 500,
                    Message = "Something went wrong."
                };

                await httpContext.Response.WriteAsJsonAsync(responseObject);
            }
        }
        
        
    }
}
