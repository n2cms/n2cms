using N2.Configuration;
using N2.Tests.Fakes;
using N2.Tests.Web.Items;
using NUnit.Framework;
using N2.Persistence;
using N2.Web;

namespace N2.Tests.Web
{
	public abstract class ParserTestsBase : ItemPersistenceMockingBase
	{
        protected FakeWebContextWrapper wrapper;
		protected IItemNotifier notifier;
		protected IHost host;
		protected IUrlParser parser;
		protected PageItem startItem, item1, item1_1, item2, item2_1;
		protected DataItem data1, data2, data3;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			wrapper = new FakeWebContextWrapper("http://www.n2cms.com/");

			notifier = mocks.Stub<IItemNotifier>();
			host = new Host(wrapper, 1, 1);
            
            parser = CreateUrlParser();
		}

        protected virtual UrlParser CreateUrlParser()
        {
            return new UrlParser(persister, wrapper, notifier, host, new HostSection());
        }

		protected void CreateDefaultStructure()
		{
			startItem = CreateOneItem<PageItem>(1, "root", null);
			item1 = CreateOneItem<PageItem>(2, "item1", startItem);
			item1_1 = CreateOneItem<PageItem>(3, "item1_1", item1);
			item2 = CreateOneItem<PageItem>(4, "item2", startItem);
			item2_1 = CreateOneItem<PageItem>(5, "item2_1", item2);

			data1 = CreateOneItem<DataItem>(6, "data1", startItem);
			data2 = CreateOneItem<DataItem>(7, "data2", item2);
			data3 = CreateOneItem<DataItem>(8, "data3", item2_1);
		}
	}
}
