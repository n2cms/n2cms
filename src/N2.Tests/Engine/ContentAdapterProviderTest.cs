using System.Collections.Generic;
using System.Linq;
using N2.Engine;
using N2.Tests.Engine.Items;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;
using N2.Engine.MediumTrust;
using N2.Engine.Castle;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class WindsorContentAdapterProviderTest : ContentAdapterProviderTest
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			provider = new ContentAdapterProvider(new ContentEngine(), new AppDomainTypeFinder());
			provider.Start();
		}
	}

	[TestFixture]
	public class MediumTrustContentAdapterProviderTest : ContentAdapterProviderTest
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			provider = new ContentAdapterProvider(new ContentEngine(new MediumTrustServiceContainer(), EventBroker.Instance, new ContainerConfigurer()), new AppDomainTypeFinder());
			provider.Start();
		}

	}

	public abstract class ContentAdapterProviderTest : ItemPersistenceMockingBase
	{
		protected ContentItem aItem, aaItem, aaaItem, abItem, aabItem;
		FakeWebContextWrapper webContext;
		protected ContentAdapterProvider provider;
			
		public override void SetUp()
		{
			base.SetUp();

			webContext = new FakeWebContextWrapper("http://www.n2cms.com/");
			
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
			var adapters = provider.Adapters.ToList();
			int aIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemA));
			int aaIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemAA));
			int aaaIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemAAA));

			Assert.That(aIndex, Is.GreaterThan(aaIndex));
			Assert.That(aaIndex, Is.GreaterThan(aaaIndex));
		}

		[Test]
		public void ResolvesAdapter_OfItemA()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(aItem.GetType());

			Assert.That(adapter, Is.TypeOf(typeof(AdapterA)));
		}

		[Test]
		public void ResolvesAdapter_OfItemAA()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(aaItem.GetType());

			Assert.That(adapter, Is.TypeOf(typeof(AdapterAA)));
		}

		[Test]
		public void ResolvesAdapter_OfItemAAA()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(aaaItem.GetType());

			Assert.That(adapter, Is.TypeOf(typeof(AdapterAAA)));
		}

		[Test]
		public void ResolvesAdapter_OfParentItemAdapter()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(abItem.GetType());

			Assert.That(adapter, Is.TypeOf(typeof(AdapterA)));
		}

		[Test]
		public void ResolvesAdapter_OfMostRelevant_ParentItemAdapter()
		{
			RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(aabItem.GetType());

			Assert.That(adapter, Is.TypeOf(typeof(AdapterAA)));
		}
	}
}
