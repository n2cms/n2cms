using N2.Tests.Content;
using NUnit.Framework;
using System;

namespace N2.Tests
{
    [Obsolete]
    [TestFixture]
    public class NodeTests : ItemTestsBase
    {
        [Test]
        public void BuildPath()
        {
            AnItem root, start;
            var rootNode = root = CreateOneItem<AnItem>(1, "root", null);
            var startNode = start = CreateOneItem<AnItem>(2, "start", root);
            var pageNode = CreateOneItem<AnItem>(3, "page", start);

            Assert.AreEqual("/", rootNode.Path);
            Assert.AreEqual("/start/", startNode.Path);
            Assert.AreEqual("/start/page/", pageNode.Path);
        }
    }
}
