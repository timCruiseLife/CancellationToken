using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Example.Web.Models
{
    public class RequestMonitorMiddleware
    {
        private class RequestCounter
        {
            private int _counter;

            internal RequestCounter(int initialValue = 0)
            {
                _counter = initialValue;
            }

            internal int Increase()
            {
                return Interlocked.Increment(ref _counter);
            }

            internal int Decrease()
            {
                return Interlocked.Decrement(ref _counter);
            }
        }

        private class TrafficTracker
        {
            private long _ts;

            private readonly ConcurrentDictionary<string, RequestCounter> _counters = new ConcurrentDictionary<string, RequestCounter>();

            internal int RecordRequest(string ip)
            {
                long num = Environment.TickCount64 / 1000;
                if (Interlocked.Exchange(ref _ts, num) != num)
                {
                    _counters.Clear();
                }

                RequestCounter orAdd = _counters.GetOrAdd(ip, (string key) => new RequestCounter());
                return orAdd.Increase();
            }
        }

        private readonly RequestMonitorOptions _options;

        private readonly RequestDelegate _next;

        private readonly ConcurrentDictionary<string, TrafficTracker> _trafficTrackers = new ConcurrentDictionary<string, TrafficTracker>();

        public RequestMonitorMiddleware(RequestDelegate next, IOptions<RequestMonitorOptions> options)
        {
            _next = next;
            _options = options?.Value ?? new RequestMonitorOptions();
        }

        public async Task Invoke(HttpContext context)
        {
            if (_options.ThrottlingEnabled && _options.Throttling.TryGetValue((string)context.Request.Path, out var value))
            {
                TrafficTracker orAdd = _trafficTrackers.GetOrAdd((string)context.Request.Path, (string key) => new TrafficTracker());
                if (orAdd.RecordRequest(context.RemoteAddr()) > value)
                {
                    context.Response.StatusCode = 403;
                    return;
                }
            }

            CancellationTokenSource cts = new CancellationTokenSource(_options.Timeout);
            try
            {
                using (context.RequestAborted.Register(delegate
                {
                    cts.Cancel();
                }))
                {
                    context.Items[typeof(CallContext)] = new CallContext(cts.Token, context.TraceIdentifier);
                    await _next(context);
                }
            }
            finally
            {
                if (cts != null)
                {
                    ((IDisposable)cts).Dispose();
                }
            }
        }
    }
}
