using System.Collections.Generic;
using System.Linq;
using N2.Engine;
using N2.Tests.Engine.Items;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class ContentAdapterProviderTest : ItemPersistenceMockingBase
	{
		protected ContentItem aItem, aaItem, aaaItem, abItem, aabItem;
		FakeWebContextWrapper webContext;
		ContentAdapterProvider provider;
			
		public override void SetUp()
		{
			base.SetUp();

			webContext = new FakeWebContextWrapper("http://www.n2cms.com/");
			provider = new ContentAdapterProvider(null, new AppDomainTypeFinder());
			provider.Start();

			aItem = CreateOneItem<ItemA>(0, "root", null);
			aaItem = CreateOneItem<ItemAA>(0, "aa", aItem);
			aaaItem = CreateOneItem<ItemAAA>(0, "aaa", aItem);
			abItem = CreateOneItem<ItemAB>(0, "ab", aItem);
			aabItem = CreateOneItem<ItemAAB>(0, "aab", aItem);
		}

		[Test]
		public void ListedDescriptors_AreSorted_FromDeepestHierarchy_ToShallowest()
		{
			List<IAdapterDescriptor> descriptors = new List<IAdapterDescriptor>();
			descriptors.Add(new ControlsAttribute(typeof(ItemA)));
			descriptors.Add(new ControlsAttribute(typeof (ItemAA)));

			descriptors.Sort();

			Assert.That(descriptors[0].ItemType, Is.EqualTo(typeof(ItemAA)));
			Assert.That(descriptors[1].ItemType, Is.EqualTo(typeof(ItemA)));
		}

		[Test]
		public void ListedDescriptors_AreChanged_WhenAlreadySorted_FromDeepestHierarchy_ToShallowest()
		{
			List<IAdapterDescriptor> descriptors = new List<IAdapterDescriptor>();
			descriptors.Add(new ControlsAttribute(typeof(ItemAA)));
			descriptors.Add(new ControlsAttribute(typeof(ItemA)));

			descriptors.Sort();

			Assert.That(descriptors[0].ItemType, Is.EqualTo(typeof (ItemAA)));
			Assert.That(descriptors[1].ItemType, Is.EqualTo(typeof(ItemA)));
		}

		[Test]
		public void Adapters_AreSorted_AccordingToInheritanceDepth()
		{
			var descriptors = provider.AdapterDescriptors.ToList();
			int aIndex = descriptors.FindIndex(d => d.ItemType == typeof(ItemA));
			int aaIndex = descriptors.FindIndex(d => d.ItemType == typeof(ItemAA));
			int aaaIndex = descriptors.FindIndex(d => d.ItemType == typeof(ItemAAA));

			Assert.That(aIndex, Is.GreaterThan(aaIndex));
			Assert.That(aaIndex, Is.GreaterThan(aaaIndex));
		}

		[Test]
		public void ResolvesAdapter_OfItemA()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(new PathData(aItem, "/default.aspx"));

			Assert.That(adapter, Is.TypeOf(typeof(AdapterA)));
		}

		[Test]
		public void ResolvesAdapter_OfItemAA()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(new PathData(aaItem, "/default.aspx"));

			Assert.That(adapter, Is.TypeOf(typeof(AdapterAA)));
		}

		[Test]
		public void ResolvesAdapter_OfItemAAA()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(new PathData(aaaItem, "/default.aspx"));

			Assert.That(adapter, Is.TypeOf(typeof(AdapterAAA)));
		}

		[Test]
		public void ResolvesAdapter_OfParentItemAdapter()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(new PathData(abItem, "/default.aspx"));

			Assert.That(adapter, Is.TypeOf(typeof(AdapterA)));
		}

		[Test]
		public void ResolvesAdapter_OfMostRelevant_ParentItemAdapter()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(new PathData(aabItem, "/default.aspx"));

			Assert.That(adapter, Is.TypeOf(typeof(AdapterAA)));
		}
	}
}
