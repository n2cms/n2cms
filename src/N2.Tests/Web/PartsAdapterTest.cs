using System.Collections.Generic;
using System.Linq;
using N2.Configuration;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using N2.Tests.Web.Items;
using N2.Web;
using N2.Web.Parts;
using NUnit.Framework;
using N2.Definitions;

namespace N2.Tests.Web
{
	[TestFixture]
	public class PartsAdapterTest : ItemPersistenceMockingBase
	{
		ContentItem pageItem, customItem, dataItem;
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
			parser = new UrlParser(persister, webContext, new NotifyingInterceptor(), new Host(webContext, pageItem.ID, pageItem.ID), hostSection);
			engine = new FakeEngine();
			AppDomainTypeFinder finder = new AppDomainTypeFinder();
            engine.AddComponentInstance(null, typeof(IDefinitionManager), new DefinitionManager(new DefinitionBuilder(finder, new EngineSection()), new N2.Workflow.StateChanger(), null));
			ContentAdapterProvider provider = new ContentAdapterProvider(engine, new AppDomainTypeFinder());
			provider.Start();
			dispatcher = new RequestDispatcher(provider, webContext, parser, new ErrorHandler(webContext, null, null), hostSection);
			engine.AddComponentInstance("", typeof(IContentAdapterProvider), provider);
		}



		[Test]
		public void CanResolve_ZoneAspectController()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>();

			Assert.That(controller, Is.TypeOf(typeof(PageZoneAdapter)));
		}

		[Test]
		public void Retrieves_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/item4");
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(customItem, "Zone1");

			Assert.That(items.Count(), Is.EqualTo(1));
		}

		[Test]
		public void CanFilter_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(pageItem, "ZoneNone");

			Assert.That(items.Count(), Is.EqualTo(0));
		}

		[Test]
		public void CanAddTo_ItemsInZone()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>();

			IEnumerable<ContentItem> items = controller.GetItemsInZone(pageItem, "ZoneAll");

			Assert.That(items.Count(), Is.EqualTo(2));
		}

		[Test]
		public void CanResolve_PossibleChildren()
		{
			webContext.CurrentPath = dispatcher.ResolveUrl("/");
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>();

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
			dataItem = CreateOneItem<DataItem>(6, "data6", customItem);
			dataItem.ZoneName = "Zone2";
			CreateOneItem<DataItem>(7, "nested7", dataItem).ZoneName = "Zone2";
			CreateOneItem<DataItem>(8, "nested8", dataItem).ZoneName = "Zone2";
		}
	}
}
