using System.Linq;
using System.Web;
using System.Xml.Linq;
using N2.Web.UI.WebControls;
using NUnit.Framework;

namespace N2.Tests.Web.WebControls
{
    [TestFixture]
    public class ControlPanelTests_DragDropState : WebControlTestsBase
    {
        [TestFixtureSetUp]
        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Initialize("edit=drag");
        }

        [Test]
        public void CanRenderItem_InDroppableZone_WhenDragDrop()
        {
            N2.Context.Current.Definitions.GetDefinitions();

            Zone z = new DroppableZone().AddedToFakePage(HttpContext.Current, page);
            z.CurrentItem = page;
            z.ZoneName = ZoneName;

            z.EnsureChildControls();

            string html = z.RenderToString();
            XDocument document = XDocument.Parse(html);
            var dropPointValues = document.Descendants("div")
                .Where(div => div.Attribute("class").ToString().Contains("zoneItem"))
                .Select(div => div.Value);
            
            Assert.That(dropPointValues.Count(), Is.EqualTo(1), "Expected one zone item node to be present.");
            Assert.That(dropPointValues.Single().Contains("[data]"));
        }
    }
}
