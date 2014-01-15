using System.Web;
using N2.Edit;
using N2.Web.UI.WebControls;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class PluginContextTests
    {
        [Test]
        public void CanEvaluate()
        {
            PluginContext ctx = new PluginContext(new SelectionUtility(new Items.CustomExtensionPage(), null), null, null, ControlPanelState.Visible, null, null);

            var result = ctx.Format("{Selected.ID}", false);
            Assert.That(result, Is.EqualTo("0"));
        }

        [Test]
        public void CanEvaluate_Selected_AsSelected_ItemPath()
        {
            PluginContext ctx = new PluginContext(new SelectionUtility(new Items.CustomExtensionPage(), null), null, null, ControlPanelState.Visible, null, null);

            var result = ctx.Format("Selected: {selected}", false);
            Assert.That(result, Is.EqualTo("Selected: /"));
        }

        [Test]
        public void CanEvaluate_Selected_AsSelected_AndUrlEncoded_ItemPath()
        {
            PluginContext ctx = new PluginContext(new SelectionUtility(new Items.CustomExtensionPage(), null), null, null, ControlPanelState.Visible, null, null);

            var result = ctx.Format("Selected: {selected}", true);
            Assert.That(result, Is.EqualTo("Selected: " + HttpUtility.UrlEncode("/")));
        }
    }
}
