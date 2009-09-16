using System;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using N2.Engine;
using N2.Extensions.Tests.Mvc.Controllers;
using N2.Extensions.Tests.Mvc.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ContentControllerTests
	{
		[Test]
		public void TakesCareOfPartsRenderedWithView()
		{
			var controller = new TestItemController();
			controller.CurrentItem = new TestItem();

			controller.UsingView().AssertResultIs<PartialViewResult>();
		}

		[Test]
		public void ReturnsViewWhenIndexCalledOnPageController()
		{
			var controller = new RegularControllerBase();
			controller.CurrentItem = new RegularPage();

			controller.Index().AssertResultIs<ViewResult>();
		}

		[Test]
		public void ReturnsPartialWhenIndexCalledOnPartController()
		{
			var controller = new TestItemController();
			controller.CurrentItem = new TestItem();

			controller.Index().AssertResultIs<PartialViewResult>();
		}

		[Test]
		public void PartsRenderWithNonContentItemModels()
		{
			var controller = new TestItemController();
			controller.CurrentItem = new TestItem();

			controller.WithModel().AssertResultIs<PartialViewResult>();
		}

		[Test]
		public void ParentPage()
		{
			var page = new RegularPage();
			var controller = new TestItemController();
			controller.CurrentItem = new TestItem
			{
				Parent = page,
			};

			Assert.That(controller.CurrentPage, Is.EqualTo(page));
		}

		[Test]
		public void ParentPage_MultipleLevel()
		{
			var page = new RegularPage();
			var controller = new TestItemController();
			controller.CurrentItem = new TestItem
			{
				Parent = new TestItem{Parent = page},
			};

			Assert.That(controller.CurrentPage, Is.EqualTo(page));
		}

		[Test]
		public void ViewParentPage_WithPage()
		{
			var page = new RegularPage();
			var controller = new RegularControllerBase();
			controller.CurrentItem = page;

			controller.Engine = MockRepository.GenerateStub<IEngine>();
			
			Assert.Throws<InvalidOperationException>(() => controller.ViewParentPage());
		}

		[Test]
		public void ViewParentPage_WithItem()
		{
			var page = new RegularPage();
			var controller = new TestItemController();
			controller.CurrentItem = new TestItem
			{
				Parent = new TestItem { Parent = page },
			};
			controller.Engine = MockRepository.GenerateStub<IEngine>();
			var result = controller.ViewParentPage();

			Assert.That(result.Page, Is.EqualTo(page));
		}

		[Test]
		public void ViewPage_WithSamePage()
		{
			var page = new RegularPage();
			var controller = new RegularControllerBase();
			controller.CurrentItem = page;
			controller.Engine = MockRepository.GenerateStub<IEngine>();

			Assert.Throws<InvalidOperationException>(() => controller.ViewPage(page));
		}
	}
}
