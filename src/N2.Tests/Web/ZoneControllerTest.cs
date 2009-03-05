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
using N2.Web.Parts;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using N2.Definitions;

namespace N2.Tests.Web
{
	[TestFixture]
	public class ZoneControllerTest : ItemPersistenceMockingBase
	{
		ContentItem pageItem, customItem;
		UrlParser parser;
		FakeWebContextWrapper webContext;
		RequestDispatcher dispatcher;
		IEngine engine;

		public override void SetUp()
		{
			base.SetUp();

			CreateDefaultStructure();
			webContext = new FakeWebContextWrapper("http://www.n2cms.com/");
			HostSection hostSection = new HostSection();
			hostSection.Web.Extension = "/";
			parser = new UrlParser(persister, webContext, new ItemNotifier(), new Host(webContext, pageItem.ID, pageItem.ID), hostSection);
			engine = new FakeEngine();
			AppDomainTypeFinder finder = new AppDomainTypeFinder();
			engine.AddComponentInstance(null, typeof (IDefinitionManager), new DefinitionManager(new DefinitionBuilder(finder), null));
			dispatcher = new RequestDispatcher(engine, webContext, parser, finder, new ErrorHandler(webContext, null, null), hostSection);
			dispatcher.Start();
		}



		[Test]
		public void CanResolve_ZoneAspectController()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			PartsAspectController controller = dispatcher.ResolveAspectController<PartsAspectController>();

			Assert.That(controller, Is.TypeOf(typeof(PageZoneController)));
		}

		[Test]
		public void Retrieves_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/item4");
			PartsAspectController controller = dispatcher.ResolveAspectController<PartsAspectController>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(customItem, "Zone1");

			Assert.That(items.Count(), Is.EqualTo(1));
		}

		[Test]
		public void CanFilter_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			PartsAspectController controller = dispatcher.ResolveAspectController<PartsAspectController>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(pageItem, "ZoneNone");

			Assert.That(items.Count(), Is.EqualTo(0));
		}

		[Test]
		public void CanAddTo_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			PartsAspectController controller = dispatcher.ResolveAspectController<PartsAspectController>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(pageItem, "ZoneAll");

			Assert.That(items.Count(), Is.EqualTo(2));
		}

		[Test]
		public void CanResolve_PossibleChildren()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			PartsAspectController controller = dispatcher.ResolveAspectController<PartsAspectController>();

			IEnumerable<ItemDefinition> items = controller.GetAllowedDefinitions(webContext.CurrentPage, "Zone1", null);

			Assert.That(items.Count(), Is.GreaterThan(0));
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
