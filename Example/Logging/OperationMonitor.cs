using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Example.Logging
{
    public sealed class OperationMonitor : IDisposable
    {
        private bool Disposed
        {
            get;
            set;
        } = false;


        private ILogger Logger
        {
            get;
            set;
        }

        private string Operation
        {
            get;
            set;
        }

        private string TraceActivityId
        {
            get;
            set;
        }

        private EventId EventId
        {
            get;
            set;
        }

        private Stopwatch Stopwatch
        {
            get;
            set;
        }

        public Exception? Exception
        {
            get;
            set;
        }

        public OperationMonitor(ILogger logger, EventId eventId, string operation, string traceActivityId)
        {
            Logger = (logger ?? throw new ArgumentNullException("logger"));
            Operation = (operation ?? throw new ArgumentNullException("operation"));
            TraceActivityId = (traceActivityId ?? string.Empty);
            EventId = eventId;
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                Stopwatch.Stop();
                long elapsedMilliseconds = Stopwatch.ElapsedMilliseconds;
                if (Exception == null)
                {
                    Logger.LogInformation(EventId, "OpMon:{eventId},{Op},{traceId},{latency}", EventId, Operation, TraceActivityId, elapsedMilliseconds);
                }
                else
                {
                    Logger.LogError(EventId, Exception, "OpMon:{eventId},{Op},{traceId},{latency}", EventId, Operation, TraceActivityId, elapsedMilliseconds);
                }
            }
        }
    }
}
