using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Configuration;
using N2.Engine;
using N2.Persistence;
using N2.Tests.Fakes;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Web
{
	[Controls(typeof(PageItem))]
	public class PageZoneController : ZoneController
	{

	}
	[Controls(typeof(CustomItem))]
	public class CustomZoneController : ZoneController
	{

	}

	[TestFixture]
	public class ZoneControllerTest : ItemPersistenceMockingBase
	{
		PageItem startItem, item1;
		ContentItem custom2;
		UrlParser parser;
		FakeWebContextWrapper webContext;
		RequestDispatcher dispatcher;

		public override void SetUp()
		{
			base.SetUp();

			CreateDefaultStructure();
			webContext = new FakeWebContextWrapper("http://www.n2cms.com/");
			HostSection hostSection = new HostSection();
			hostSection.Web.Extension = "/";
			parser = new UrlParser(persister, webContext, new ItemNotifier(), new Host(webContext, startItem.ID, startItem.ID), hostSection);
			dispatcher = new RequestDispatcher(null, parser, webContext, new AppDomainTypeFinder(), new ErrorHandler(webContext, null, null), hostSection);
			dispatcher.Start();
		}



		[Test]
		public void CanFilterItems_ThroughZoneController()
		{
			var path = dispatcher.ResolveUrl("/");
			IZoneController controller = dispatcher.ResolveAspectController<IZoneController>(path);

			Assert.That(controller, Is.TypeOf(typeof(PageZoneController)));
		}



		protected void CreateDefaultStructure()
		{
			startItem = CreateOneItem<PageItem>(1, "root", null);
			item1 = CreateOneItem<PageItem>(2, "item1", startItem);
			custom2 = CreateOneItem<CustomItem>(3, "custom2", startItem);
		}
	}
}
