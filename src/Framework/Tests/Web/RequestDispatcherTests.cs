//using N2.Configuration;
//using N2.Edit.Versioning;
//using N2.Engine;
//using N2.Engine.MediumTrust;
//using N2.Persistence.Serialization;
//using N2.Tests.Fakes;
//using N2.Tests.Web.Items;
//using N2.Web;
//using NUnit.Framework;

//namespace N2.Tests.Web
//{
//    [TestFixture]
//    public class WindsorRequestDispatcherTests : RequestDispatcherTests
//    {
//        [SetUp]
//        public override void SetUp()
//        {
//            base.SetUp();

//            var provider = new ContentAdapterProvider(new ContentEngine(), new AppDomainTypeFinder());
//            provider.Start();
//            dispatcher = new RequestPathProvider(webContext, parser, new FakeErrorHandler(), hostSection, versionRepository);
//        }
//    }

//    [TestFixture]
//    public class MediumTrustRequestDispatcherTests : RequestDispatcherTests
//    {
//        [SetUp]
//        public override void SetUp()
//        {
//            base.SetUp();

//            ContentAdapterProvider provider = new ContentAdapterProvider(new ContentEngine(new MediumTrustServiceContainer(), EventBroker.Instance, new ContainerConfigurer()), new AppDomainTypeFinder());
//            provider.Start();
//            dispatcher = new RequestPathProvider(webContext, parser, new FakeErrorHandler(), hostSection, versionRepository);
//        }
//    }
    
//    public abstract class RequestDispatcherTests : ItemPersistenceMockingBase
//    {
//        protected PageItem startItem, item1, item1_1, item2, item2_1;
//        protected ContentItem custom3, particular4, special5, other6;
//        protected HostSection hostSection;
//        protected UrlParser parser;
//        protected FakeWebContextWrapper webContext;
//        protected RequestPathProvider dispatcher;
//        protected ContentVersionRepository versionRepository;

//        public override void SetUp()
//        {
//            base.SetUp();
            
//            CreateDefaultStructure();
//            webContext = new FakeWebContextWrapper("http://www.n2cms.com/");
//            hostSection = new HostSection {Web = new WebElement {ObserveEmptyExtension = true}};
//            parser = new UrlParser(persister, webContext, new Host(webContext, startItem.ID, startItem.ID), new N2.Plugin.ConnectionMonitor(), hostSection);
//            versionRepository = TestSupport.CreateVersionRepository(ref persister, typeof(PageItem));
//        }

//        //[Test]
//        //public void CanResolve_DefaultRequestController()
//        //{
//        //    SetUrl("/");

//        //    RequestAdapter adapter = dispatcher.ResolveAdapter<RequestAdapter>();

//        //    Assert.That(adapter, Is.TypeOf(typeof(RequestAdapter)));
//        //}

//        //[Test]
//        //public void CanResolve_CustomController()
//        //{
//        //    SetUrl("/custom3.aspx");

//        //    RequestAdapter adapter = dispatcher.ResolveAdapter<RequestAdapter>();

//        //    Assert.That(adapter, Is.TypeOf(typeof(CustomRequestAdapter)));
//        //}

//        //[Test]
//        //public void CanResolve_CustomController_OnInheritedItem()
//        //{
//        //    SetUrl("/particular4.aspx");

//        //    RequestAdapter adapter = dispatcher.ResolveAdapter<RequestAdapter>();

//        //    Assert.That(adapter, Is.TypeOf(typeof(CustomRequestAdapter)));
//        //}

//        //[Test]
//        //public void CanResolve_CustomController_WithRedefinedController()
//        //{
//        //    SetUrl("/special5.aspx");

//        //    RequestAdapter adapter = dispatcher.ResolveAdapter<RequestAdapter>();

//        //    Assert.That(adapter, Is.TypeOf(typeof(SpecialCustomAdapter)));
//        //}

//        //[Test]
//        //public void CanResolve_CustomController_WithOtherController()
//        //{
//        //    SetUrl("/other6.aspx");

//        //    RequestAdapter adapter = dispatcher.ResolveAdapter<RequestAdapter>();

//        //    Assert.That(adapter, Is.TypeOf(typeof(OtherCustomController)));
//        //}

//        protected void CreateDefaultStructure()
//        {
//            startItem = CreateOneItem<PageItem>(1, "root", null);
//            item1 = CreateOneItem<PageItem>(2, "item1", startItem);
//            item1_1 = CreateOneItem<PageItem>(3, "item1_1", item1);
//            item2 = CreateOneItem<PageItem>(4, "item2", startItem);
//            item2_1 = CreateOneItem<PageItem>(5, "item2_1", item2);
//            custom3 = CreateOneItem<CustomItem>(6, "custom3", startItem);
//            particular4 = CreateOneItem<ParticularCustomItem>(7, "particular4", startItem);
//            special5 = CreateOneItem<SpecialCustomItem>(8, "special5", startItem);
//            other6 = CreateOneItem<OtherCustomItem>(9, "other6", startItem);
//        }

//        void SetUrl(string url)
//        {
//            webContext.Url = url;
//            webContext.CurrentPath = parser.ResolvePath(url);
//        }
//    }
//}
