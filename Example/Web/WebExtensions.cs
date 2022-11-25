using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;

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

    internal static bool IsClosed(this WebSocket webSocket)
    {
        return webSocket.CloseStatus.HasValue || webSocket.State == WebSocketState.None || webSocket.State == WebSocketState.Aborted || webSocket.State == WebSocketState.CloseReceived || webSocket.State == WebSocketState.CloseSent || webSocket.State == WebSocketState.Closed;
    }
}