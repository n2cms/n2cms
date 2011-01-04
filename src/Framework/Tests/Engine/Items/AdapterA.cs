using N2.Web;
using N2.Engine;

namespace N2.Tests.Engine.Items
{
	[Adapts(typeof(ItemA))]
	public class AdapterA : RequestAdapter
	{
	}
}