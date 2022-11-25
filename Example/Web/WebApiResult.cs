using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Example.Web
{
    public class WebApiResult<T>
    {
        public string Error
        {
            get;
            set;
        } = string.Empty;


        public string ErrorMessage
        {
            get;
            set;
        } = string.Empty;


        public string TraceActivity
        {
            get;
            set;
        } = string.Empty;


        public T? Result
        {
            get;
            set;
        }

        public WebApiResult()
        {
        }

        public WebApiResult(T result)
            : this("success", result)
        {
        }

        public WebApiResult(string error, T result)
        {
            Error = (error ?? "success");
            Result = result;
        }

        public WebApiResult(string traceActivity, Exception exception)
        {
            TraceActivity = traceActivity;
            Error = exception.GetType().Name;
            ErrorMessage = exception.Message;
        }

        public WebApiResult(string traceActivity, string error, string errorMessage)
        {
            TraceActivity = traceActivity;
            Error = error;
            ErrorMessage = errorMessage;
        }
    }
}
