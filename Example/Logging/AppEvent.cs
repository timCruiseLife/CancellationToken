using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Example.Logging
{
    public static class AppEvent
    {
        public static class Db
        {
            public static readonly EventId GetOne = new EventId(1001);

            public static readonly EventId GetAll = new EventId(1002);

            public static readonly EventId Execute = new EventId(1003);

            public static readonly EventId ExecuteScalar = new EventId(1004);

            public static readonly EventId BeginTrans = new EventId(1005);

            public static readonly EventId Commit = new EventId(1006);

            public static readonly EventId Rollback = new EventId(1007);
        }

        public static class Request
        {
            public static readonly EventId Success = new EventId(2001);

            public static readonly EventId ClientError = new EventId(2002);

            public static readonly EventId ServerError = new EventId(2003);
        }

        public static class Middleware
        {
            public static readonly EventId DnsFailed = new EventId(3001);

            public static readonly EventId NoTenantMatched = new EventId(3002);

            public static readonly EventId BadTenantCookie = new EventId(3003);

            public static readonly EventId WebApiFailed = new EventId(3011);

            public static readonly EventId WsAuthFailed = new EventId(3021);

            public static readonly EventId WsAuthzFailed = new EventId(3022);

            public static readonly EventId WsNoListener = new EventId(3023);

            public static readonly EventId WsListenerNoRoute = new EventId(3024);

            public static readonly EventId WsListenerError = new EventId(3025);

            public static readonly EventId WsListenerLoaded = new EventId(3026);

            public static readonly EventId WsBadMessage = new EventId(3027);

            public static readonly EventId WsProcessFailed = new EventId(3028);
        }

        internal static class DCluster
        {
            internal static readonly EventId GenerateId = new EventId(4001);

            internal static readonly EventId TransactionNotFound = new EventId(4002);

            internal static readonly EventId TransactionInDeterminedState = new EventId(4003);

            internal static readonly EventId BadTransactionIdFormat = new EventId(4004);

            internal static readonly EventId InvalidParticipant = new EventId(4005);

            internal static readonly EventId TransactionFixed = new EventId(4006);

            internal static readonly EventId GeneratorUpsertError = new EventId(4601);

            internal static readonly EventId XTransactionError = new EventId(4602);

            internal static readonly EventId FailedToFixTransaction = new EventId(4603);
        }

        public static readonly EventId IncomingRequest = new EventId(8001);

        public static readonly EventId OutgoingRequest = new EventId(8002);
    }
}
