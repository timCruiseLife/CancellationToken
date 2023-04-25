namespace Example.Web.Models
{
    public class RequestMonitorOptions
    {
        /// <summary>
        /// request time out to trigger cancellationToken
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1.0);

        /// <summary>
        /// set up the number of requests must be less than the limit
        /// </summary>
        public Dictionary<string, int> Throttling { get; private set; } = new Dictionary<string, int>();

        internal bool ThrottlingEnabled => Throttling.Count > 0;
    }
}