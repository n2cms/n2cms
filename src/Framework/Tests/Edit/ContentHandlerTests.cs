using N2.Configuration;
using N2.Edit;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Management.Api;
using N2.Persistence;
using N2.Security;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2.Tests.Edit
{
	public abstract class ContentHandlerTests
	{
		protected Fakes.FakeEngine engine;
		protected ContentHandler handler;
		protected Fakes.FakeWebContextWrapper context;
		protected ContentHandlerTestsPage startPage;
		protected ContentHandlerTestsPage page;
		protected VersionManager versionManager;

		protected class ContentHandlerTestsPage : ContentItem
		{
		}

		[PartDefinition]
		protected class ContentHandlerTestsPart : ContentItem
		{
		}

		[SetUp]
		public void SetUp()
		{
			engine = new Fakes.FakeEngine(new Type[] { typeof(ContentHandlerTestsPage), typeof(ContentHandlerTestsPart), typeof(Fakes.FakeNodeAdapter) });
			//engine.Resolve<Fakes.FakeNodeAdapter>().Engine = engine;
			handler = new ContentHandler(engine);
			context = new Fakes.FakeWebContextWrapper();
			context.HttpContext.User = SecurityUtilities.CreatePrincipal("Admin");

			startPage = new ContentHandlerTestsPage { Title = "Start page" };
			engine.Persister.Save(startPage);
			page = new ContentHandlerTestsPage { Title = "Page in question" };
			page.AddTo(startPage);
			engine.Persister.Save(page);
			
			engine.AddComponentInstance<IWebContext>(context);
			engine.AddComponentInstance<IUrlParser>(new Fakes.FakeUrlParser(startPage: startPage));
			var persister = engine.Persister;
			var activator = engine.Resolve<ContentActivator>();
			var versionRepository = TestSupport.CreateVersionRepository(ref persister, ref activator, new Type[] { typeof(ContentHandlerTestsPage), typeof(ContentHandlerTestsPart) });
			engine.AddComponentInstance<ContentVersionRepository>(versionRepository);
			engine.AddComponentInstance<IVersionManager>(versionManager = TestSupport.SetupVersionManager(engine.Persister, versionRepository));
			(engine.Resolve<IContentAdapterProvider>() as N2.Plugin.IAutoStart).Start();
			engine.Resolve<IContentAdapterProvider>().ResolveAdapter<N2.Edit.NodeAdapter>(typeof(ContentItem)).Engine = engine;
			engine.AddComponentInstance(new HtmlSanitizer(new N2.Configuration.HostSection()));
			engine.AddComponentInstance<IEditUrlManager>(new FakeEditUrlManager());
			engine.AddComponentInstance(new ConfigurationManagerWrapper());
			engine.AddComponentInstance<ILanguageGateway>(new FakeLanguageGateway());

			engine.AddComponentInstance(new DraftRepository(versionRepository, new FakeCacheWrapper()));
		}

	}
}
