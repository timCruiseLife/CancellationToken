using System.Net.WebSockets;
using Example.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Example.Web;

public static class WebExtentions
{
    public static string TraceActivity(this ControllerBase controller)
    {
        return controller.HttpContext.TraceIdentifier;
    }

    public static CallContext GetCallContext(this ControllerBase controller)
    {
        return controller.HttpContext.GetCallContext();
    }

    public static WebApiResult<T> CreateErrorResult<T>(this ControllerBase controller, string error, string errorMessage)
    {
        return new WebApiResult<T>(controller.TraceActivity(), error, errorMessage);
    }

    public static void UseRequestLogging(this IApplicationBuilder applicationBuilder, params string[] excludingPrefixes)
    {
        LoggingOptions loggingOptions = new LoggingOptions();
        if (excludingPrefixes != null)
        {
            foreach (string prefix in excludingPrefixes)
            {
                loggingOptions.AddExcludingPrefix(prefix);
            }
        }

        applicationBuilder.UseMiddleware<LoggingMiddleware>(new object[1] { Options.Create(loggingOptions) });
    }

    public static void UseRequestMonitor(this IApplicationBuilder applicationBuilder, Action<RequestMonitorOptions>? configAction = null)
    {
        RequestMonitorOptions requestMonitorOptions = new RequestMonitorOptions();
        configAction?.Invoke(requestMonitorOptions);
        applicationBuilder.UseMiddleware<RequestMonitorMiddleware>(new object[1] { Options.Create(requestMonitorOptions) });
    }

    public static void UseRequestMonitor(this IApplicationBuilder applicationBuilder, TimeSpan requestTimeout)
    {
        applicationBuilder.UseRequestMonitor(delegate (RequestMonitorOptions options)
        {
            options.Timeout = requestTimeout;
        });
    }

    internal static bool IsClosed(this WebSocket webSocket)
    {
        return webSocket.CloseStatus.HasValue || webSocket.State == WebSocketState.None || webSocket.State == WebSocketState.Aborted || webSocket.State == WebSocketState.CloseReceived || webSocket.State == WebSocketState.CloseSent || webSocket.State == WebSocketState.Closed;
    }
}