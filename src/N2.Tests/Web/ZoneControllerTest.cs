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
	[TestFixture]
	public class ZoneControllerTest : ItemPersistenceMockingBase
	{
		ContentItem pageItem, customItem;
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
			parser = new UrlParser(persister, webContext, new ItemNotifier(), new Host(webContext, pageItem.ID, pageItem.ID), hostSection);
			dispatcher = new RequestDispatcher(null, webContext, parser, new AppDomainTypeFinder(), new ErrorHandler(webContext, null, null), hostSection);
			dispatcher.Start();
		}



		[Test]
		public void CanResolve_ZoneAspectController()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			ZoneAspectController controller = dispatcher.ResolveAspectController<ZoneAspectController>();

			Assert.That(controller, Is.TypeOf(typeof(PageZoneController)));
		}

		[Test]
		public void ZoneAspectController_Retrieves_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/item4");
			ZoneAspectController controller = dispatcher.ResolveAspectController<ZoneAspectController>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(customItem, "Zone1");

			Assert.That(items.Count(), Is.EqualTo(1));
		}

		[Test]
		public void ZoneAspectController_CanFilter_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			ZoneAspectController controller = dispatcher.ResolveAspectController<ZoneAspectController>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(pageItem, "ZoneNone");

			Assert.That(items.Count(), Is.EqualTo(0));
		}

		[Test]
		public void ZoneAspectController_CanAddTo_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			ZoneAspectController controller = dispatcher.ResolveAspectController<ZoneAspectController>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(pageItem, "ZoneAll");

			Assert.That(items.Count(), Is.EqualTo(2));
		}

		protected void CreateDefaultStructure()
		{
			pageItem = CreateOneItem<PageItem>(1, "root", null);
			CreateOneItem<DataItem>(2, "data2", pageItem).ZoneName = "Zone1";
			CreateOneItem<DataItem>(3, "data3", pageItem).ZoneName = "Zone2";

			customItem = CreateOneItem<CustomItem>(4, "item4", pageItem);
			CreateOneItem<DataItem>(5, "data5", customItem).ZoneName = "Zone1";
			CreateOneItem<DataItem>(6, "data6", customItem).ZoneName = "Zone2";
		}
	}
}
