using System;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using N2.Templates.Mvc.Controllers;
using N2.Templates.Mvc.Tests.Items;
using NUnit.Framework;

namespace N2.Templates.Mvc.Tests.Controllers
{
	/// <summary>
	/// Summary description for HomeControllerTest
	/// </summary>
	[TestFixture]
	public class HomeControllerTest
	{
		[Test]
		public void Index()
		{
			// Setup
			var controller = new HomeController {CurrentItem = new FakeContentItem()};

			// Execute
			var result = controller.Index().AssertViewRendered();

			// Verify
			var item = result.WithViewData<FakeContentItem>();

			Assert.That(item, Is.EqualTo(controller.CurrentItem));
		}
	}
}
