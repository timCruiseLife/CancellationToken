namespace Example.Web
{
    public static class HttpContextExtensions
    {
        public static string RemoteAddr(this HttpContext context)
        {
            if (context.Items.TryGetValue("RemoteAddr", out object? value))
            {
                return value as string ?? string.Empty;
            }

            return (context.Connection?.RemoteIpAddress?.ToString() ?? string.Empty);
        }

        public static string RequestScheme(this HttpContext context)
        {
            if (context.Items.TryGetValue("RequestScheme", out object? value))
            {
                return value as string ?? string.Empty;
            }
            return string.Empty;
        }

        public static CallContext GetCallContext(this HttpContext context)
        {
            if (context.Items.TryGetValue(typeof(CallContext), out object? value))
            {
                return value as CallContext ?? CallContext.Default;
            }

            return CallContext.Default;
        }
    }
}