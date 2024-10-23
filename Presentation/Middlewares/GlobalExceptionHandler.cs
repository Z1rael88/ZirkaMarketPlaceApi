using Application.Exceptions;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

                (int statusCode, string message, ICollection<string> errors) = HandleException(ex);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                ExceptionResponse exceptionResponse = new() { Errors = errors, Message = message };

                await context.Response.WriteAsJsonAsync(exceptionResponse);
            }
        }

        private (int StatusCode, string Message, ICollection<string> Errors) HandleException(Exception ex)
        {
            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "Server error";
            ICollection<string> errors = new List<string>();

            switch (ex)
            {
                case DbUpdateException dbUpdateException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    if (dbUpdateException.InnerException != null)
                    {
                        errors = new List<string> { dbUpdateException.InnerException.Message };
                    }

                    break;
                case InvalidOperationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case IdentityException identityEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    errors = identityEx.Errors;
                    message = identityEx.Message;
                    break;
                case ArgumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    errors.Add(ex.Message);
                    break;
                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = ex.Message;
                    break;

            }

            return (statusCode, message, errors);
        }
    }
}