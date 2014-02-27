using N2.Engine;
using N2.Web;

namespace N2.Tests.Web.Items
{
    [Adapts(typeof(CustomItem))]
    public class CustomRequestAdapter : RequestAdapter
    {
    }
}
