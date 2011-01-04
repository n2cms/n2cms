using N2.Web;
using N2.Engine;

namespace N2.Tests.Web.Items
{
	[Adapts(typeof(SpecialCustomItem))]
	public class SpecialCustomAdapter : RequestAdapter
	{
	}
}