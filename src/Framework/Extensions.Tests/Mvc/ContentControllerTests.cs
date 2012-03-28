using System;
using System.Web.Mvc;
using N2.Engine;
using N2.Extensions.Tests.Mvc.Controllers;
using N2.Extensions.Tests.Mvc.Models;
using N2.Tests.Fakes;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ContentControllerTests
	{
		//[Test, Ignore("TODO: Reconsider this")]
		//public void TakesCareOfPartsRenderedWithView()
		//{
		//    var controller = Create<TestItemController>();
		//    controller.CurrentItem = new TestItem();

		//    controller.UsingView().AssertResultIs<PartialViewResult>();
		//}

		[Test]
		public void ReturnsViewWhenIndexCalledOnPageController()
		{
			var controller = Create<RegularController>();
			controller.CurrentItem = new RegularPage();

			Assert.That(controller.Index(), Is.InstanceOf<ViewResult>());
		}

		//[Test, Ignore("TODO: Reconsider this")]
		//public void ReturnsPartialWhenIndexCalledOnPartController()
		//{
		//    var controller = Create<TestItemController>();
		//    controller.CurrentItem = new TestItem();

		//    controller.Index().AssertResultIs<PartialViewResult>();
		//}

		//[Test, Ignore("TODO: Reconsider this")]
		//public void PartsRenderWithNonContentItemModels()
		//{
		//    var controller = new TestItemController();
		//    controller.CurrentItem = new TestItem();

		//    controller.WithModel().AssertResultIs<PartialViewResult>();
		//}

		[Test]
		public void ParentPage()
		{
			var page = new RegularPage();
			var controller = Create<TestItemController>();
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
			var controller = Create<TestItemController>();
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
			var controller = Create<RegularController>();
			controller.CurrentItem = page;

			controller.Content.Current.Engine = MockRepository.GenerateStub<IEngine>();
			
			Assert.Throws<InvalidOperationException>(() => controller.ViewParentPage());
		}

		[Test]
		public void ViewParentPage_WithItem()
		{
			var page = new RegularPage();
			var controller = Create<TestItemController>();
			controller.CurrentItem = new TestItem
			{
				Parent = new TestItem { Parent = page },
			};
			controller.Content.Current.Engine = MockRepository.GenerateStub<IEngine>();
			var result = controller.ViewParentPage();

			Assert.That(result.Page, Is.EqualTo(page));
		}

		[Test]
		public void ViewPage_WithSamePage()
		{
			var page = new RegularPage();
			var controller = Create<RegularController>();
			controller.CurrentItem = page;
			controller.Content.Current.Engine = MockRepository.GenerateStub<IEngine>();

			Assert.Throws<InvalidOperationException>(() => controller.ViewPage(page));
		}

		private T Create<T>() where T : Controller, new()
		{
			T c = new T();
			c.ControllerContext = new ControllerContext(new System.Web.Routing.RequestContext(new FakeHttpContext(), new System.Web.Routing.RouteData()), c);
			return c;
		}
	}
}
