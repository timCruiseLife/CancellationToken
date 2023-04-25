using Microsoft.AspNetCore.Http;

namespace Example.Web.Models
{
    public class LoggingOptions
    {
        public List<PathString> ExcludingPrefixes { get; } = new List<PathString>();

        public void AddExcludingPrefix(string prefix)
        {
            ExcludingPrefixes.Add(PathString.FromUriComponent(prefix));
        }
    }
}