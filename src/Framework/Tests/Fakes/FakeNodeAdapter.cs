using N2.Edit;
using N2.Engine;

namespace N2.Tests.Fakes
{
	[Adapts(typeof(ContentItem))]
	public class FakeNodeAdapter : NodeAdapter
	{
		public FakeNodeAdapter()
		{
		}
	}
}