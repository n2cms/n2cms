using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI;
using N2.Web.UI.WebControls;
using NUnit.Framework;
using N2.Web.UI;

namespace N2.Tests.Web.WebControls
{
    [TestFixture]
    public class ControlPanelTests_HiddenState : WebControlTestsBase
    {
        [TestFixtureSetUp]
        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();
            Initialize("");
        }

        [Test]
        public void CanRenderItem_InZone()
        {
            Zone z = new Zone().AddedToFakePage(HttpContext.Current, page);
            z.CurrentItem = page;
            z.ZoneName = "TheZone";

            z.EnsureChildControls();

            string html = z.RenderToString();
            Assert.That(html, Is.EqualTo("[data]"));
        }

        [Test]
        public void CanRenderItem_InDroppableZone()
        {
            Zone z = new DroppableZone().AddedToFakePage(HttpContext.Current, page);
            z.CurrentItem = page;
            z.ZoneName = "TheZone";

            z.EnsureChildControls();

            string html = z.RenderToString();
            Assert.That(html, Is.EqualTo("[data]"));
        }

        [Test]
        public void CanRenderItem_InDroppableZone_WhenDragDrop()
        {
            Zone z = new DroppableZone().AddedToFakePage(HttpContext.Current, page);
            z.CurrentItem = page;
            z.ZoneName = "TheZone";

            z.EnsureChildControls();

            string html = z.RenderToString();
            Assert.That(html, Is.EqualTo("[data]"));
        }
    }

    public static class ControlExtensions
    {
        public static T AddedToFakePage<T>(this T control, HttpContext context, ContentItem item)
            where T : Control
        {
            var p = new ContentPage();
            p.Set("_request", context.Request);
            p.CurrentPage = item;
            control.Page = p;

            return control;
        }

        public static void Set(this object instance, string memberName, object value)
        {
            var field = instance.GetType().GetField(memberName, BindingFlags.NonPublic | BindingFlags.Instance);
            if(field != null)
                field.SetValue(instance, value);
            else
                instance.GetType().GetProperty(memberName, BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(instance, value, null);
        }

        public static void EnsureChildControls(this Control control)
        {
            var method = typeof(Control).GetMethod("EnsureChildControls", BindingFlags.NonPublic | BindingFlags.Instance);
            try
            {
                method.Invoke(control, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public static string RenderToString(this Control control)
        {
            using(var sw = new StringWriter())
            using(var htw = new HtmlTextWriter(sw))
            {
                control.RenderControl(htw);
                return sw.ToString();
            }
        }
    }
}
