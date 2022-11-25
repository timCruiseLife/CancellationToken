using Microsoft.AspNetCore.Http;

namespace Example.Web
{
    public static class HttpContextExtensions
    {
        public static string RemoteAddr(this HttpContext context)
        {
            object? value;
            return context.Items.TryGetValue("RemoteAddr", out value) ? ((string)value) : (context.Connection?.RemoteIpAddress?.ToString() ?? string.Empty);
        }

        public static string RequestScheme(this HttpContext context)
        {
            object? value;
            return context.Items.TryGetValue("RequestScheme", out value) ? ((string)value) : string.Empty;
        }

        public static CallContext GetCallContext(this HttpContext context)
        {
            object? value;
            return context.Items.TryGetValue(typeof(CallContext), out value) ? ((CallContext)value) : CallContext.Default;
        }
    }
}
