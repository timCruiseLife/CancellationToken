using System.Diagnostics;
using System.Security.Claims;
using Example.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Example.Web.Models
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly LoggingOptions? _options;

        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, IOptions<LoggingOptions> loggingOptions, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException("loggerFactory");
            }

            _next = next ?? throw new ArgumentNullException("next");
            _options = loggingOptions?.Value;
            _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (_options != null)
            {
                foreach (PathString excludingPrefix in _options!.ExcludingPrefixes)
                {
                    if (context.Request.Path.StartsWithSegments(excludingPrefix))
                    {
                        await _next(context);
                        return;
                    }
                }
            }

            Exception? error = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                error = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                string text = (context.Request.Path + context.Request.QueryString.Value) ?? string.Empty;
                string method = context.Request.Method;
                int statusCode = context.Response.StatusCode;
                string text2 = context.User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") ?? "anonymous";
                string text3 = context.RemoteAddr();
                if (statusCode >= 400 && statusCode < 500)
                {
                    _logger.LogWarning(AppEvent.Request.ClientError, "{traceId};{sourceIp};{user};{method};{path};{status};{latency}", context.TraceIdentifier, text3, text2, method, text, statusCode, stopwatch.ElapsedMilliseconds);
                }
                else if (statusCode >= 500)
                {
                    _logger.LogError(AppEvent.Request.ServerError, "{traceId};{sourceIp};{user};{method};{path};{status};{latency}", context.TraceIdentifier, text3, text2, method, text, statusCode, stopwatch.ElapsedMilliseconds);
                }
                else if (error != null)
                {
                    _logger.LogError(AppEvent.Request.ServerError, error, "{traceId};{sourceIp};{user};{method};{path};{status};{latency}", context.TraceIdentifier, text3, text2, method, text, statusCode, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogInformation("{traceId};{sourceIp};{user};{method};{path};{status};{latency}", context.TraceIdentifier, text3, text2, method, text, statusCode, stopwatch.ElapsedMilliseconds);
                }
            }
        }
    }
}