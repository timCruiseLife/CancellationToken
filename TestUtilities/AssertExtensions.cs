using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities
{
    public static class AssertExtensions
    {
        public static void AssertOk<T>(this ActionResult<T> result)
        {
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        public static void AssertNotFound<T>(this ActionResult<T> result)
        {
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        public static void AssertBadRequest<T>(this ActionResult<T> result)
        {
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        public static void AssertConflict<T>(this ActionResult<T> result)
        {
            Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));
        }

        public static void AssertNoContent<T>(this ActionResult<T> result)
        {
            Assert.IsInstanceOfType(result.Result, typeof(NoContentResult));
        }

        public static void AssertEquals(this double value, double expected)
        {
            bool condition = Math.Abs(value - expected) < 1E-07;
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new(16, 2);
            defaultInterpolatedStringHandler.AppendLiteral("expect ");
            defaultInterpolatedStringHandler.AppendFormatted(expected);
            defaultInterpolatedStringHandler.AppendLiteral(" but got ");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            Assert.IsTrue(condition, defaultInterpolatedStringHandler.ToStringAndClear());
        }

        public static void AssertEquals(this float value, float expected)
        {
            bool condition = (double)Math.Abs(value - expected) < 1E-07;
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new(16, 2);
            defaultInterpolatedStringHandler.AppendLiteral("expect ");
            defaultInterpolatedStringHandler.AppendFormatted(expected);
            defaultInterpolatedStringHandler.AppendLiteral(" but got ");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            Assert.IsTrue(condition, defaultInterpolatedStringHandler.ToStringAndClear());
        }

        public static T GetResult<T>(this ActionResult<T> result) where T : class
        {
            T? val = ((OkObjectResult)result.Result).Value as T;
            Assert.IsNotNull(val);
            return val;
        }

        public static T GetErrorResult<T>(this ActionResult<T> result) where T : class
        {
            if (result.Result is BadRequestObjectResult badRequestObjectResult)
            {
                T? val = badRequestObjectResult.Value as T;
                Assert.IsNotNull(val);
                return val;
            }

            if (result.Result is NotFoundObjectResult notFoundObjectResult)
            {
                T? val2 = notFoundObjectResult.Value as T;
                Assert.IsNotNull(val2);
                return val2;
            }

            T? val3 = ((ConflictObjectResult)result.Result).Value as T;
            Assert.IsNotNull(val3);
            return val3;
        }
    }
}