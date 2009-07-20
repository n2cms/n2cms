using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Extensions.Tests.Mvc.Controllers;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class N2ViewEngineTests : N2.Tests.ItemPersistenceMockingBase
	{
		private IViewEngine mockViewEngine;

		[SetUp]
		public override void SetUp()
		{
			PathDictionary.Instance[typeof(RegularPage)] = new[] { new MvcConventionTemplateAttribute("Regular") };

			mockViewEngine = MockRepository.GenerateMock<IViewEngine>();
		}

		[TearDown]
		public override void TearDown()
		{
			PathDictionary.Instance.Remove(typeof(RegularPage));
		}

		[Test]
		public void CanGetTemplateUrl_DefaultView()
		{
			var testViewEngine = new N2ViewEngine(mockViewEngine);
			var httpContextBase = MockRepository.GenerateStub<HttpContextBase>();
			var routeData = new RouteData();
			routeData.Values.Add(ContentRoute.ContentItemKey, new RegularPage());
			var controller = new RegularControllerBase();

			var controllerContext = new ControllerContext(httpContextBase, routeData, controller);

			mockViewEngine.Expect(engine => engine.FindView(controllerContext, "~/Views/Regular/Index.aspx", null, false))
				.Return(null).Repeat.Once();

			// Act
			testViewEngine.FindView(controllerContext, "Index", null, false);

			// Assert
			mockViewEngine.VerifyAllExpectations();
		}

		[Test]
		public void CanGetTemplateUrl_OtherView()
		{
			var testViewEngine = new N2ViewEngine(mockViewEngine);
			var httpContextBase = MockRepository.GenerateStub<HttpContextBase>();
			var routeData = new RouteData();
			routeData.Values.Add(ContentRoute.ContentItemKey, new RegularPage());
			var controller = new RegularControllerBase();

			var controllerContext = new ControllerContext(httpContextBase, routeData, controller);

			mockViewEngine.Expect(engine => engine.FindView(controllerContext, "~/Views/Regular/OtherView.aspx", null, false))
				.Return(null).Repeat.Once();

			// Act
			testViewEngine.FindView(controllerContext, "OtherView", null, false);

			// Assert
			mockViewEngine.VerifyAllExpectations();
		}
	}
}