using N2.Tests.Content;
using NUnit.Framework;

namespace N2.Tests
{
	[TestFixture]
	public class NodeTests : ItemTestsBase
	{
		[Test]
		public void BuildPath()
		{
			AnItem root, start;
			INode rootNode = root = CreateOneItem<AnItem>(1, "root", null);
			INode startNode = start = CreateOneItem<AnItem>(2, "start", root);
			INode pageNode = CreateOneItem<AnItem>(3, "page", start);

			Assert.AreEqual("/", rootNode.Path);
			Assert.AreEqual("/start/", startNode.Path);
			Assert.AreEqual("/start/page/", pageNode.Path);
		}
	}
}
