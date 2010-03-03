using System;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using N2.Engine;
using N2.Templates.Mvc.Controllers;
using N2.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Tests.Controllers
{
	/// <summary>
	/// Summary description for HomeControllerTest
	/// </summary>
	[TestFixture]
	public class HomeControllerTest
	{
		private HomeController _homeController;

		[SetUp]
		public void SetUp()
		{
			_homeController = new HomeController { CurrentItem = new StartPage() };
			new MvcContrib.TestHelper.TestControllerBuilder().InitializeController(_homeController);

			N2.Context.Replace(MockRepository.GenerateStub<IEngine>());
		}

		[Test]
		public void Index()
		{
			// Setup
			_homeController.CurrentItem = new StartPage();

			// Execute
			var result = _homeController.Index().AssertViewRendered();

			// Verify
			var item = result.WithViewData<StartPage>();

			Assert.That(item, Is.EqualTo(_homeController.CurrentItem));
		}

		[Test]
		public void NotFound()
		{
			// Setup
			var notFoundPage = new TextPage();
			_homeController.CurrentItem = new StartPage()
			                              	{
			                              		NotFoundPage = notFoundPage
			                              	};

			// Execute
			var result = _homeController.NotFound();

			// Verify
			Assert.That(result is ViewPageResult);
			Assert.That(((ViewPageResult)result).Page, Is.EqualTo(notFoundPage));
		}

		[Test]
		public void NotFound_NoNotFoundPageSet()
		{
			// Setup
			var notFoundPage = new TextPage();
			_homeController.CurrentItem = new StartPage();

			// Execute
			var result = _homeController.NotFound();

			// Verify
			Assert.That(result is ContentResult);
		}
	}
}
