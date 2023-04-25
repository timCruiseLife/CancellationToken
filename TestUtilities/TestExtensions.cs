using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestUtilities
{
    public static class TestExtensions
    {
        public static Mock<HttpContext> SetupMocks(this ControllerBase controller, params string[] roles)
        {
            return controller.SetupMocks(new Dictionary<object, object> { { "RemoteAddr", "127.0.0.1" } }, (roles == null) ? new Claim[0] : roles.Select((string item) => new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", item)).ToArray());
        }

        public static Mock<HttpContext> SetupMocks(this ControllerBase controller, Dictionary<object, object> context, params Claim[] claims)
        {
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();
            List<Claim> list = new List<Claim>();
            if (claims != null)
            {
                list.AddRange(claims);
            }

            claimsPrincipal.AddIdentity(new ClaimsIdentity(list));
            Mock<HttpContext> mock = new Mock<HttpContext>();
            Mock<HttpRequest> mock2 = new Mock<HttpRequest>();
            Mock<IHeaderDictionary> mock3 = new Mock<IHeaderDictionary>();
            Mock<IAuthenticationService> mock4 = new Mock<IAuthenticationService>();
            Mock<IServiceProvider> mock5 = new Mock<IServiceProvider>();
            Dictionary<object, object> dictionary = context ?? new Dictionary<object, object>();
            dictionary.TryAdd("RemoteAddr", "127.0.0.1");
            mock4.Setup((IAuthenticationService _) => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>())).Returns(Task.CompletedTask);
            mock5.Setup((IServiceProvider _) => _.GetService(typeof(IAuthenticationService))).Returns(mock4.Object);
            mock3.SetupGet((IHeaderDictionary m) => m["User-Agent"]).Returns("UnitTest");
            mock2.SetupGet((HttpRequest m) => m.Headers).Returns(mock3.Object);
            mock.SetupGet((HttpContext m) => m.Items).Returns(dictionary);
            mock.SetupGet((HttpContext m) => m.Request).Returns(mock2.Object);
            mock.SetupGet((HttpContext m) => m.RequestServices).Returns(mock5.Object);
            mock.SetupGet((HttpContext m) => m.User).Returns(claimsPrincipal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mock.Object
            };
            return mock;
        }

        public static Mock<HttpContext> SetupMocks(this ControllerBase controller, long id, params string[] roles)
        {
            return controller.SetupMocksWithIdentifier(id.ToString(), roles);
        }

        public static Mock<HttpContext> SetupMocksWithIdentifier(this ControllerBase controller, string identifier, params string[] roles)
        {
            List<Claim> list = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", identifier)
            };
            if (roles != null && roles.Length != 0)
            {
                list.AddRange(roles.Select((string item) => new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", item)));
            }

            return controller.SetupMocks(new Dictionary<object, object> { { "RemoteAddr", "127.0.0.1" } }, list.ToArray());
        }
    }
}