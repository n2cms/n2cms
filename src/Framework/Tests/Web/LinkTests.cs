using System.Web.UI.HtmlControls;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class LinkTests : ItemTestsBase
    {
        [Test]
        public void CanCreateLinkString()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            string anchor = Link.To(item).ToString();

            Assert.AreEqual("<a href=\"/yoda\">yoda</a>", anchor);
        }

        [Test]
        public void CanCreateLinkControl()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            HtmlAnchor anchor = (HtmlAnchor)Link.To(item).ToControl();

            Assert.AreEqual(anchor.InnerHtml, "yoda");
            Assert.AreEqual(anchor.HRef, "/yoda");
            Assert.AreEqual(anchor.Title, "");
            Assert.AreEqual(anchor.Target, "");
        }

        [Test]
        public void CanCreateLinkStringWithTargetAndTitle()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            string anchor = Link.To(item)
                .Title("open in new window")
                .Target("_top")
                .ToString();

            Assert.AreEqual("<a href=\"/yoda\" target=\"_top\" title=\"open in new window\">yoda</a>", anchor);
        }

        [Test]
        public void CanCreateLinkControlWithTargetAndTitle()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            HtmlAnchor anchor = (HtmlAnchor)Link.To(item)
                .Title("open in new window")
                .Target("_top")
                .ToControl();

            Assert.AreEqual(anchor.InnerHtml, "yoda");
            Assert.AreEqual(anchor.HRef, "/yoda");
            Assert.AreEqual(anchor.Title, "open in new window");
            Assert.AreEqual(anchor.Target, "_top");
        }

        [Test]
        public void LinkWithoutHrefBecomesSpan()
        {
            string anchor = Link.To(null).Text("No link isn't it?").ToString();
            Assert.AreEqual("<span>No link isn't it?</span>", anchor);
        }

        [Test]
        public void CanCreateEmptyLink()
        {
            string anchor = Link.To(null).ToString();
            Assert.AreEqual("", anchor);
        }

        [Test]
        public void CanSetQueryString()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            string anchor = Link.To(item).Query("hungry=yes").ToString();

            Assert.AreEqual("<a href=\"/yoda?hungry=yes\">yoda</a>", anchor);
        }

        [Test]
        public void CanAddQueryString()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            string anchor = Link.To(item).AddQuery("hungry", "yes").ToString();

            Assert.AreEqual("<a href=\"/yoda?hungry=yes\">yoda</a>", anchor);
        }

        [Test]
        public void CanSetAndAdd_QueryStrings()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            string anchor = Link.To(item).Query("feedingTime=soon").AddQuery("hungry", "yes").ToString();

            Assert.AreEqual("<a href=\"/yoda?feedingTime=soon&amp;hungry=yes\">yoda</a>", anchor);
        }

        [Test]
        public void CanAdd_Multiple_QueryStrings()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            string anchor = Link.To(item).AddQuery("feedingTime", "soon").AddQuery("hungry", "yes").ToString();

            Assert.AreEqual("<a href=\"/yoda?feedingTime=soon&amp;hungry=yes\">yoda</a>", anchor);
        }

        [Test]
        public void CanRemove_QueryStrings()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            string anchor = Link.To(item).Query("feedingTime=soon&hungry=yes").AddQuery("hungry", null).ToString();

            Assert.AreEqual("<a href=\"/yoda?feedingTime=soon\">yoda</a>", anchor);
        }

        [Test]
        public void Attributes_AreRendered()
        {
            PageItem item = CreateOneItem<PageItem>(1, "yoda", null);

            string anchor = Link.To(item).Attribute("rel", "/yoda").ToString();

            Assert.AreEqual("<a href=\"/yoda\" rel=\"/yoda\">yoda</a>", anchor);
        }
    }
}
