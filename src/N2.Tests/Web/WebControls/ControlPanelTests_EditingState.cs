using System.Web.UI;
using N2.Web.UI.WebControls;
using NUnit.Framework;

namespace N2.Tests.Web.WebControls
{
	[TestFixture]
	public class ControlPanelTests_EditingState : WebControlTestsBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Initialize("edit=true");
		}

		[Test]
		public void CanRenderItem_InDroppableZone_WhenDragDrop()
		{
			Zone z = new DroppableZone();
			z.CurrentItem = page;
			z.ZoneName = ZoneName;

			Page p = new Page();
			p.Controls.Add(z);

			z.EnsureChildControls();

			string html = z.RenderToString();
			Assert.That(html, Is.EqualTo("[data]"));
		}
	}
}
