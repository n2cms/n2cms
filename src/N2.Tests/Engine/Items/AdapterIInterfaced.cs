using N2.Web;
using N2.Engine;

namespace N2.Tests.Engine.Items
{
	[Adapts(typeof(IInterfacedItem))]
	public class AdapterIInterfaced : RequestAdapter
	{
	}
}