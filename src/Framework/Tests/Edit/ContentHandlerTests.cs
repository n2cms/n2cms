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
	[TestFixture]
	public class ContentHandlerTests
	{
		private Fakes.FakeEngine engine;
		private ContentHandler handler;
		private Fakes.FakeWebContextWrapper context;
		private ContentHandlerTestsPage startPage;
		private ContentHandlerTestsPage page;
		private VersionManager versionManager;

		class ContentHandlerTestsPage : ContentItem
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
			engine.AddComponentInstance<VersionManager>(versionManager = TestSupport.SetupVersionManager(engine.Persister, TestSupport.CreateVersionRepository(new Type[] { typeof(ContentHandlerTestsPage) })));
		}

		[Test]
		public void AutoSave_CreatesDraft()
		{
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + page.ID, "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State != ContentState.Draft).Title.ShouldBe("Page in question");
			versions.Single(v => v.State == ContentState.Draft).Title.ShouldBe("New title");
		}
	}
}
