using N2.Engine;
using N2.Web;

namespace N2.Tests.Engine.Items
{
    [Adapts(typeof(IInterfacedItem))]
    public class AdapterIInterfaced : RequestAdapter
    {
    }
}
