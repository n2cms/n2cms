using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;
using N2.Web.UI.WebControls;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Web.WebControls
{
	[TestFixture]
	public class ControlPanelTests_DragDropState : WebControlTestsBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Initialize("edit=drag");
		}

		[Test]
		public void CanRenderItem_InDroppableZone_WhenDragDrop()
		{
			N2.Context.Current.Definitions.GetDefinitions();

			Zone z = new DroppableZone();
			z.CurrentItem = page;
			z.ZoneName = ZoneName;

			Page p = new Page();
			p.Controls.Add(z);

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
