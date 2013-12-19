using System.Web;
using N2.Web.UI.WebControls;
using NUnit.Framework;

namespace N2.Tests.Web.WebControls
{
    [TestFixture]
    public class ControlPanelTests_EditingState : WebControlTestsBase
    {
        [TestFixtureSetUp]
        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Initialize("edit=true");
        }

        [Test]
        public void CanRenderItem_InDroppableZone_WhenDragDrop()
        {
            Zone z = new DroppableZone().AddedToFakePage(HttpContext.Current, page);
            z.CurrentItem = page;
            z.ZoneName = ZoneName;

            z.EnsureChildControls();

            string html = z.RenderToString();
            Assert.That(html, Is.EqualTo("[data]"));
        }
    }
}
