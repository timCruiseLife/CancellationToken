namespace Example.Web
{
    public class CallContext
    {
        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        public string TraceActivityId
        {
            get;
            set;
        }

        public static CallContext Default
        {
            get;
        } = new CallContext();

        public CallContext()
            : this(CancellationToken.None, string.Empty)
        {
        }

        public CallContext(CancellationToken cancellationToken, string traceActivityId)
        {
            CancellationToken = cancellationToken;
            TraceActivityId = String.IsNullOrEmpty(traceActivityId) ? Guid.NewGuid().ToString() : traceActivityId;
        }
    }
}