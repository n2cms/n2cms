using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Tests;
using N2.Tests.Fakes;
using N2.Persistence.Finder;
using N2.Web;
using Rhino.Mocks;
using N2.Persistence;
using N2.Details;
using N2.Management.Externals;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Web.Mvc;
using N2.Engine;
using N2.Edit;

namespace N2.Management.Tests.ExternalContent
{
	[TestFixture]
	public class ExternalContentTests
	{
		Externals.ExternalContentRepository externalRepository;
		ContentItem root;
		ContentItem start;
		FakeHttpContext context;
		RequestContext rc;

		[SetUp]
		public void SetUp()
		{
			externalRepository = ExternalContentRepositoryTests.SetupRepository(out root, out start);
			var rd = new RouteData();
			rd.Values["controller"] = "stub";
			rd.Values["action"] = "index";
			var engine = MockRepository.GenerateStub<IEngine>();
			engine.Stub(e => e.Resolve<IExternalContentRepository>()).Return(externalRepository);
			rd.DataTokens[ContentRoute.ContentEngineKey] = engine;
			context = new FakeHttpContext();
			rc = new RequestContext(context, rd);
		}

		[Test]
		public void ExternalItem_IsAssociated_WithoutKey()
		{
			IController controller = new StubController();
			controller.Execute(rc);

			var external = rc.RouteData.DataTokens["externalItem"] as ExternalItem;
			Assert.That(external, Is.Not.Null);
			Assert.That(external.Name, Is.EqualTo(ExternalItem.SingleItemKey));
			Assert.That(external.ZoneName, Is.EqualTo("stub"));
		}

		[Test]
		public void ExternalItem_IsAssociated_ToRouteKey()
		{
			rc.RouteData.Values["id"] = "1";
			rc.RouteData.Values["controller"] = "stub2";

			IController controller = new Stub2Controller();
			controller.Execute(rc);

			var external = rc.RouteData.DataTokens["externalItem"] as ExternalItem;
			Assert.That(external, Is.Not.Null);
			Assert.That(external.Name, Is.EqualTo("1"));
			Assert.That(external.ZoneName, Is.EqualTo("stub2"));
		}

		[N2.Web.Mvc.ExternalContent]
		class StubController : Controller
		{
			public ActionResult Index()
			{
				RouteData.DataTokens["externalItem"] = RouteData.CurrentPage();
				return Content("");
			}
		}

		[N2.Web.Mvc.ExternalContent("id")]
		class Stub2Controller : Controller
		{
			public ActionResult Index()
			{
				RouteData.DataTokens["externalItem"] = RouteData.CurrentPage();
				return Content("");
			}
		}
	}
}
