using System.Web;
using N2.Web.Mvc;

namespace N2.Tests.Fakes
{
    public class FakeHttpServerUtility : HttpServerUtilityBase
    {
        public override void Execute(IHttpHandler handler, System.IO.TextWriter writer, bool preserveForm)
        {
            var requestContext = ((System.Web.Mvc.MvcHandler)handler.GetType().GetField("_httpHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(handler)).RequestContext;
            writer.Write(requestContext.RouteData.Values.ToQueryString());
        }
    }
}
