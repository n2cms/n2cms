using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using N2.Engine;
using N2.Engine.Castle;
using N2.Tests.Web.Items;
using NUnit.Framework;
using N2.Web;

namespace N2.Tests.Web.WebControls
{
    public abstract class WebControlTestsBase : ItemTestsBase
    {
        protected const string ZoneName = "TheZone";
        
        protected PageItem page;
        protected DataItem data;
        private ContentEngine engine;

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            engine = new ContentEngine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None),
                "n2nodb",
                new WindsorServiceContainer(),
                new EventBroker(),
                new ContainerConfigurer());
            N2.Context.Replace(engine);
            engine.Initialize();
            engine.Host.CurrentSite.RootItemID = 1;
            engine.Host.CurrentSite.StartPageID = 1;
        }
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            page = CreateOneItem<PageItem>(1, "page", null);
            data = CreateOneItem<DataItem>(2, "data", page);
            data.ZoneName = ZoneName;
            engine.RequestContext.CurrentPath = new PathData(page);
        }
        
        protected override T CreateOneItem<T>(int id, string name, ContentItem parent)
        {
            var item = base.CreateOneItem<T>(id, name, parent);
            N2.Context.Current.Persister.Repository.SaveOrUpdate(item);
            return item;
        }

        protected void Initialize(string queryString)
        {
            var request = new HttpRequest("/Default.aspx", "http://localhost/", queryString);
            request.Browser = new HttpBrowserCapabilities();
            request.Browser.Capabilities = new Hashtable();
            request.Browser.Capabilities["ecmascriptversion"] = "1.7";
            request.Browser.Capabilities["w3cdomversion"] = "2.0";
            var response = new HttpResponse(new StringWriter(new StringBuilder()));
            HttpContext.Current = new HttpContext(request, response)
            {
                ApplicationInstance = new HttpApplication(), 
                User = SecurityUtilities.CreatePrincipal("admin")
            };
            HttpContext.Current.Items["N2.Engine"] = engine;
            System.Threading.Thread.CurrentPrincipal = HttpContext.Current.User;
            ((ThreadContext)engine.RequestContext).Url = request.RawUrl;
        }
    }
}
