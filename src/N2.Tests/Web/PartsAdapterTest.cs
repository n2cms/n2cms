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
using N2.Engine.MediumTrust;

namespace N2.Tests.Web
{
	[TestFixture]
	public class WindsorPartsAdapterTest : PartsAdapterTest
	{
		[SetUp]
		public override void SetUp()
		{
			engine = new ContentEngine();
			base.SetUp();
		}
	}

	[TestFixture]
	public class MediumTrustPartsAdapterTest : PartsAdapterTest
	{
		[SetUp]
		public override void SetUp()
		{
			engine = new ContentEngine(new MediumTrustServiceContainer(), EventBroker.Instance, new ContainerConfigurer());
			//engine.Resolve<EngineSection>().Assemblies.Add(new System.Web.Configuration.AssemblyInfo("N2"));
			//engine.Resolve<EngineSection>().Assemblies.Add(new System.Web.Configuration.AssemblyInfo("N2.Tests"));
			base.SetUp();
		}
	}

	public abstract class PartsAdapterTest : ItemPersistenceMockingBase
	{
		ContentItem pageItem, customItem, dataItem;
		IContentAdapterProvider dispatcher;
		protected IEngine engine;

		public override void SetUp()
		{
			base.SetUp();

			CreateDefaultStructure();
			
			((ContentAdapterProvider)engine.Resolve<IContentAdapterProvider>()).Start();
			dispatcher = engine.Resolve<IContentAdapterProvider>();
		}

		// /					pageItem	(PageItem)
		// /data2							(DataItem)
		// /data3							(DataItem)
		// /item4				customItem	(PageItem)
		// /item4/data5			dataItem	(DataItem)
		// /item4/data5/nested7				(DataItem)
		// /item4/data5/nested7				(DataItem)

		[Test]
		public void CanResolve_ZoneAdapter()
		{
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>(pageItem);

			Assert.That(controller, Is.TypeOf(typeof(PageZoneAdapter)));
		}

		[Test]
		public void Retrieves_ItemsInZone()
		{
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>(customItem);

			IEnumerable<ContentItem> items = controller.GetItemsInZone(customItem, "Zone1");

			Assert.That(items.Count(), Is.EqualTo(1));
		}

		[Test]
		public void CanFilter_ItemsInZone()
		{
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>(pageItem);

			IEnumerable<ContentItem> items = controller.GetItemsInZone(pageItem, "ZoneNone");

			Assert.That(items.Count(), Is.EqualTo(0));
		}

		[Test]
		public void CanAddTo_ItemsInZone()
		{
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>(pageItem);

			IEnumerable<ContentItem> items = controller.GetItemsInZone(pageItem, "ZoneAll");

			Assert.That(items.Count(), Is.EqualTo(2));
		}

		[Test]
		public void CanResolve_PossibleChildren()
		{
			PartsAdapter controller = dispatcher.ResolveAdapter<PartsAdapter>(pageItem);

			IEnumerable<ItemDefinition> items = controller.GetAllowedDefinitions(pageItem, "Zone1", null);

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
