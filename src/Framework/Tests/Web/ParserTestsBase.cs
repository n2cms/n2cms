using N2.Configuration;
using N2.Tests.Fakes;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    public abstract class ParserTestsBase : ParserTestsBase<PageItem, DataItem>
    {
    }

    public abstract class ParserTestsBase<TPage, TPart> : ItemPersistenceMockingBase
        where TPage : ContentItem
        where TPart : ContentItem
    {
        protected FakeWebContextWrapper wrapper;
        protected IHost host;
        protected IUrlParser parser;
        protected TPage startItem, page1, page1_1, page2, page2_1;
        protected TPart part1, part2, part3;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            wrapper = new FakeWebContextWrapper("http://www.n2cms.com/");

            host = new Host(wrapper, 1, 1);
            
            parser = CreateUrlParser();
        }

        protected virtual UrlParser CreateUrlParser()
        {
            return TestSupport.Setup(persister, wrapper, host);
        }

        protected void CreateDefaultStructure()
        {
            startItem = CreateOneItem<TPage>(1, "root", null);
            page1 = CreateOneItem<TPage>(2, "item1", startItem);
            page1_1 = CreateOneItem<TPage>(3, "item1_1", page1);
            page2 = CreateOneItem<TPage>(4, "item2", startItem);
            page2_1 = CreateOneItem<TPage>(5, "item2_1", page2);

            part1 = CreateOneItem<TPart>(6, "data1", startItem);
            part2 = CreateOneItem<TPart>(7, "data2", page2);
            part3 = CreateOneItem<TPart>(8, "data3", page2_1);
        }
    }
}
