using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence;
using N2.Web;

namespace N2.Tests.Web
{
	public abstract class ParserTestsBase : ItemPersistenceMockingBase
	{
		protected IWebContext wrapper;
		protected IItemNotifier notifier;
		protected Site site;
		protected UrlParser parser;
		protected PageItem startItem, item1, item1_1, item2, item2_1;
		protected DataItem data1, data2, data3;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			wrapper = CreateWrapper(true);

			notifier = mocks.Stub<IItemNotifier>();
			site = new Site(1);
		}

		protected void CreateItems(bool replay)
		{
			startItem = CreateOneItem<PageItem>(1, "root", null);
			item1 = CreateOneItem<PageItem>(2, "item1", startItem);
			item1_1 = CreateOneItem<PageItem>(3, "item1_1", item1);
			item2 = CreateOneItem<PageItem>(4, "item2", startItem);
			item2_1 = CreateOneItem<PageItem>(5, "item2_1", item2);

			data1 = CreateOneItem<DataItem>(6, "data1", startItem);
			data2 = CreateOneItem<DataItem>(7, "data2", item2);
			data3 = CreateOneItem<DataItem>(8, "data3", item2_1);

			if(replay)
				mocks.Replay(persister);
		}
	}
}
