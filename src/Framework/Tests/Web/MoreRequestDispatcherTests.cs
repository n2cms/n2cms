using System.Configuration;
using N2.Configuration;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Tests.Fakes;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;
using N2.Persistence;
using N2.Web.Targeting;
using System.Collections.Specialized;

namespace N2.Tests.Web
{
    [TestFixture]
    public class BeginRequestMoreRequestDispatcherTests : MoreRequestDispatcherTests
    {
        public BeginRequestMoreRequestDispatcherTests()
        {
            rewriteMethod = RewriteMethod.BeginRequest;
        }

        [Test]
        public void Context_IsNot_RewrittenBack_ToFriendlyUrl_AfterMapRequestHandler()
        {
            webContext.Url = "/one/two";

            TriggerRewrite();
            handler.PostMapRequestHandler();

            Assert.That(webContext.rewrittenPath, Is.EqualTo("/Default.aspx?n2page=3"));
        }
    }

    [TestFixture]
    public class SurroundMapRequestHandlerMoreRequestDispatcherTests : MoreRequestDispatcherTests
    {
        public SurroundMapRequestHandlerMoreRequestDispatcherTests()
        {
            rewriteMethod = RewriteMethod.SurroundMapRequestHandler;
        }

        [Test]
        public void Context_IsRewrittenBack_ToFriendlyUrl_AfterMapRequestHandler()
        {
            webContext.Url = "/one/two.aspx";

            TriggerRewrite();
            handler.PostMapRequestHandler();

            Assert.That(webContext.rewrittenPath, Is.EqualTo("/one/two.aspx"));
        }
    }


    public abstract class MoreRequestDispatcherTests : ItemPersistenceMockingBase
    {
        protected FakeRequestLifeCycleHandler handler;
        IUrlParser parser;
        RequestPathProvider dispatcher;
        protected FakeWebContextWrapper webContext;
        ContentItem root, two, three;
        CustomExtensionItem one;
        IErrorNotifier errorHandler;
        IEngine engine;
        ContentAdapterProvider adapterProvider;
        protected RewriteMethod rewriteMethod = RewriteMethod.BeginRequest;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            root = CreateOneItem<PageItem>(1, "root", null);
            one = CreateOneItem<CustomExtensionItem>(2, "one", root);
            two = CreateOneItem<PageItem>(3, "two", one);
            CreateOneItem<DataItem>(4, "four", root);
            three = CreateOneItem<PageItem>(5, "three.3", root);

            webContext = new FakeWebContextWrapper();
            var hostSection = new HostSection { Web = new WebElement { Rewrite = rewriteMethod, ObservedExtensions = new StringCollection { ".aspx" } } };
            parser = new UrlParser(persister, webContext, new Host(webContext, root.ID, root.ID), new N2.Plugin.ConnectionMonitor(), hostSection);
            errorHandler = new FakeErrorHandler();
            engine = new FakeEngine();
            engine.Container.AddComponentInstance(null, typeof(IWebContext), webContext);
            engine.Container.AddComponentInstance(null, typeof(TargetingRadar), new TargetingRadar(hostSection, new DetectorBase[0]));
            adapterProvider = new ContentAdapterProvider(engine, new AppDomainTypeFinder());
            adapterProvider.Start();

            ReCreateDispatcherWithConfig(hostSection);
        }

        protected void TriggerRewrite()
        {
            handler.BeginRequest();
            handler.PostResolveRequestCache();
        }

        [Test]
        public void CanRewriteUrl()
        {
            webContext.Url = "/one/two";

            TriggerRewrite();

            Assert.That(webContext.rewrittenPath, Is.EqualTo("/Default.aspx?n2page=3"));
        }

        [Test]
        public void RewriteUrl_AppendsExistingQueryString()
        {
            webContext.Url = "/one/two?happy=true&flip=feet";

            TriggerRewrite();

            Assert.That(webContext.rewrittenPath, Is.EqualTo("/Default.aspx?happy=true&flip=feet&n2page=3"));
        }

        [Test]
        public void UpdateContentPage()
        {
            webContext.Url = "/one/two";

            TriggerRewrite();

            Assert.That(webContext.CurrentPath, Is.Not.Null);
            Assert.That(webContext.CurrentPage, Is.EqualTo(two));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsAspx()
        {
            webContext.Url = "/one.aspx";

            try
            {
                Url.DefaultExtension = ".aspx";
                TriggerRewrite();
            }
            finally
            {
                Url.DefaultExtension = "";
            }

            Assert.That(webContext.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void UpdatesCurrentPage_WhenExtension_IsConfiguredAsObserved()
        {
            one.extension = ".htm";
            ReCreateDispatcherWithConfig(new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection { ".html", ".htm" } } });
            webContext.Url = "/one.htm";

            TriggerRewrite();

            Assert.That(webContext.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void DoesntUpdateCurrentPage_WhenExtension_IsNotObserved()
        {
            ReCreateDispatcherWithConfig(new HostSection { Web = new WebElement { ObservedExtensions = new CommaDelimitedStringCollection() } });
            webContext.Url = "/one.html";

            TriggerRewrite();

            Assert.That(webContext.CurrentPage, Is.Null);
        }

        [Test]
        public void UpdatesCurrentPage_WhenAllExtensions_AreObserved_AndOddExtension_IsPassed()
        {
            ReCreateDispatcherWithConfig(new HostSection { Web = new WebElement { ObserveAllExtensions = true } });
            webContext.Url = "/three.3";

            TriggerRewrite();

            Assert.That(webContext.CurrentPage, Is.EqualTo(three));
        }

        [Test]
        public void DoesntUpdateCurrentPage_WhenExtension_IsEmpty_AndEmpty_IsNotObserved()
        {
            ReCreateDispatcherWithConfig(new HostSection { Web = new WebElement { Extension = ".aspx", ObserveEmptyExtension = false } });
            webContext.Url = "/one";

            TriggerRewrite();

            Assert.That(webContext.CurrentPage, Is.Null);
        }

        [Test]
        public void UpdatesCurrentPage_WhenEmptyExtension_IsConfiguredAsObserved()
        {
            ReCreateDispatcherWithConfig(new HostSection { Web = new WebElement { ObserveEmptyExtension = true, ObservedExtensions = new CommaDelimitedStringCollection() } });
            webContext.Url = "/one";

            TriggerRewrite();

            Assert.That(webContext.CurrentPage, Is.EqualTo(one));
        }

        [Test]
        public void UpdateContentPage_WithRewrittenUrl()
        {
            webContext.Url = "/Default.aspx?n2page=3";

            TriggerRewrite();

            Assert.That(webContext.CurrentPage, Is.EqualTo(two));
        }

        [Test]
        public void UpdateContentPage_WithItemReference_UpdatesWithPage()
        {
            webContext.Url = "/Default.aspx?n2item=4&n2page=3";

            TriggerRewrite();

            Assert.That(webContext.CurrentPage, Is.EqualTo(two));
        }

        [Test]
        public void UpdatesCurrentPage_WhenUrl_IsWebDevStartPage()
        {
            webContext.Url = "/Default.aspx?";

            TriggerRewrite();

            Assert.That(webContext.CurrentPage, Is.EqualTo(root));
        }

        void ReCreateDispatcherWithConfig(HostSection config)
        {
            IPersister persister = null;
            dispatcher = new RequestPathProvider(webContext, parser, errorHandler, config, TestSupport.CreateDraftRepository(ref persister));

            handler = new FakeRequestLifeCycleHandler(webContext, dispatcher, adapterProvider, errorHandler,
                new ConfigurationManagerWrapper { Sections = new ConfigurationManagerWrapper.ContentSectionTable(config, null, null, new EditSection()) });
        }
    }
}
