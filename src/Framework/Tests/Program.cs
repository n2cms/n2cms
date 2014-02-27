using N2.Engine;

namespace N2.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);

            IEngine engine = new ContentEngine();
            engine.Container.AddComponentInstance(typeof(IServiceContainer).FullName, typeof(IServiceContainer), engine.Container);
            engine.Container.StartComponents();
            //var cache = engine.Container.Resolve<N2.Engine.StructureBoundCache<N2.Engine.Globalization.LanguageInfo>>();
            var cache = engine.Container.Resolve<N2.Tests.Engine.Services.DependingGenericSelfService<string>>();
            

            //var c = new N2.Castle.WindsorServiceContainer();
            //c.AddComponentInstance("0", typeof(IServiceContainer), c);
            //c.AddComponentInstance("1", typeof(ITypeFinder), new FakeTypeFinder(typeof(N2.Context).Assembly, typeof(N2.Context).Assembly.GetTypes()));
            //var finder = Rhino.Mocks.MockRepository.GenerateStub<N2.Persistence.Finder.IItemFinder>();
            //c.AddComponentInstance("2", typeof(N2.Persistence.IPersister), new N2.Persistence.NH.ContentPersister(new FakeRepository<ContentItem>(), new FakeRepository<N2.Details.LinkDetail>(), finder));
            //c.AddComponent("3", typeof(N2.Engine.ServiceRegistrator), typeof(N2.Engine.ServiceRegistrator));
            //c.Resolve<N2.Engine.ServiceRegistrator>().Start();

            //var cache = c.Resolve<N2.Engine.StructureBoundDictionaryCache<int, N2.Engine.Globalization.LanguageInfo>>();

            //Persistence.PersistFixture pf = new N2.Tests.Persistence.PersistFixture();
            //pf.SetUp();
            //pf.InsertRootNode();
            //pf.TearDown();

            //Xml.N2XmlWriterFixture f = new N2.Tests.Xml.N2XmlWriterFixture();
            //f.SetUp();
            //Console.Clear();
            //f.WriteAndReadOneNodeWithDetails();
        
            //f.WriteRootNode();
            //Console.ReadLine();
            //f.TearDown();
        }
    }
}
