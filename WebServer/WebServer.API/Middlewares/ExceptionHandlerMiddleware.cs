using Newtonsoft.Json;

namespace WebServer.API.Middlewares
{
    public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, "error during executing {Context}", context.Request.Path.Value);
                HttpResponse response = context.Response;
                response.ContentType = "application/json";
                (int status, string message, string? parameter) = GetResponse(exception);
                response.StatusCode = status;
                await response.WriteAsync(JsonConvert.SerializeObject(new { Error = message }));
            }
        }

        private (int status, string message, string? parameter) GetResponse(Exception exception)
        {
            switch (exception)
            {
                case ArgumentException argumentException:
                    return (StatusCodes.Status400BadRequest, argumentException.Message, null);
                case InvalidOperationException invalidOperationException:
                    return (StatusCodes.Status400BadRequest, invalidOperationException.Message, null);
                case NullReferenceException nullReferenceException:
                    return (StatusCodes.Status400BadRequest, nullReferenceException.Message, null);
                case UnauthorizedAccessException unauthorizedAccessException:
                    return (StatusCodes.Status401Unauthorized, unauthorizedAccessException.Message, null);
                default:
                    return (StatusCodes.Status500InternalServerError, "Internal server error", null);
            }
        }
    }
}
