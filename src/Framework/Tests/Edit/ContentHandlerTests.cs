using N2.Edit.Versioning;
using N2.Management.Api;
using N2.Persistence;
using N2.Security;
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

		[SetUp]
		public void SetUp()
		{
			engine = new Fakes.FakeEngine(new Type[] { typeof(ContentHandlerTestsPage) });
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
			var versionRepository = TestSupport.CreateVersionRepository(new Type[] { typeof(ContentHandlerTestsPage) });
			engine.AddComponentInstance<ContentVersionRepository>(versionRepository);
			engine.AddComponentInstance<VersionManager>(versionManager = TestSupport.SetupVersionManager(engine.Persister, versionRepository));
		}

	}
}
