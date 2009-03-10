using System.Reflection;
using NUnit.Framework;
using N2.Web.UI.WebControls;
using System.Web.UI;
using System.Text;
using System.IO;

namespace N2.Tests.Web.WebControls
{
	[TestFixture]
	public class ControlPanelTests_HiddenState : WebControlTestsBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Initialize("");
		}

		[Test]
		public void CanRenderItem_InZone()
		{
			Zone z = new Zone();
			z.CurrentItem = page;
			z.ZoneName = "TheZone";

			Page p = new Page();
			p.Controls.Add(z);

			z.EnsureChildControls();

			string html = z.RenderToString();
			Assert.That(html, Is.EqualTo("[data]"));
		}

		[Test]
		public void CanRenderItem_InDroppableZone()
		{
			Zone z = new DroppableZone();
			z.CurrentItem = page;
			z.ZoneName = "TheZone";

			Page p = new Page();
			p.Controls.Add(z);

			z.EnsureChildControls();

			string html = z.RenderToString();
			Assert.That(html, Is.EqualTo("[data]"));
		}

		[Test]
		public void CanRenderItem_InDroppableZone_WhenDragDrop()
		{
			Zone z = new DroppableZone();
			z.CurrentItem = page;
			z.ZoneName = "TheZone";

			Page p = new Page();
			p.Controls.Add(z);

			z.EnsureChildControls();

			string html = z.RenderToString();
			Assert.That(html, Is.EqualTo("[data]"));
		}
	}

	public static class ControlExtensions
	{
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
			StringBuilder sb = new StringBuilder();
			using(var sw = new StringWriter(sb))
			using(var htw = new HtmlTextWriter(sw))
			{
				control.RenderControl(htw);
			}
			return sb.ToString();
		}
	}
}
