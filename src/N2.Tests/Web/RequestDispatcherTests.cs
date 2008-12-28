using N2.Persistence;
using N2.Tests.Fakes;
using NUnit.Framework;
using N2.Web;
using NUnit.Framework.SyntaxHelpers;
using N2.Tests.Web.Items;
using N2.Engine;

namespace N2.Tests.Web.FrontDispatcherTests
{
	[TestFixture]
	public class RequestDispatcherTests : ItemPersistenceMockingBase
	{
		protected PageItem startItem, item1, item1_1, item2, item2_1;
		protected ContentItem custom3, particular4, special5, other6;
		UrlParser parser;
		FakeWebContextWrapper webContext;
		RequestDispatcher dispatcher;
			
		public override void SetUp()
		{
			base.SetUp();

			CreateDefaultStructure();

			webContext = new FakeWebContextWrapper("http://www.n2cms.com/");
			parser = new UrlParser(persister, webContext, new ItemNotifier(), new Host(webContext, startItem.ID, startItem.ID));
			dispatcher = new RequestDispatcher(parser, webContext, new AppDomainTypeFinder());
			dispatcher.Start();
		}

		[Test]
		public void CanResolve_DefaultController()
		{
			webContext.Url = "/";

			BaseController controller = dispatcher.ResolveController();

			Assert.That(controller, Is.TypeOf(typeof(BaseController)));
		}

		[Test]
		public void CanResolve_CustomController()
		{
			webContext.Url = "/custom3.aspx";

			BaseController controller = dispatcher.ResolveController();

			Assert.That(controller, Is.TypeOf(typeof(CustomController)));
		}

		[Test]
		public void CanResolve_CustomController_OnInheritedItem()
		{
			webContext.Url = "/particular4.aspx";

			BaseController controller = dispatcher.ResolveController();

			Assert.That(controller, Is.TypeOf(typeof(CustomController)));
		}

		[Test]
		public void CanResolve_CustomController_WithRedefinedController()
		{
			webContext.Url = "/special5.aspx";

			BaseController controller = dispatcher.ResolveController();

			Assert.That(controller, Is.TypeOf(typeof(SpecialCustomController)));
		}

		[Test]
		public void CanResolve_CustomController_WithOtherController()
		{
			webContext.Url = "/other6.aspx";

			BaseController controller = dispatcher.ResolveController();

			Assert.That(controller, Is.TypeOf(typeof(OtherCustomController)));
		}

		protected void CreateDefaultStructure()
		{
			startItem = CreateOneItem<PageItem>(1, "root", null);
			item1 = CreateOneItem<PageItem>(2, "item1", startItem);
			item1_1 = CreateOneItem<PageItem>(3, "item1_1", item1);
			item2 = CreateOneItem<PageItem>(4, "item2", startItem);
			item2_1 = CreateOneItem<PageItem>(5, "item2_1", item2);
			custom3 = CreateOneItem<CustomItem>(6, "custom3", startItem);
			particular4 = CreateOneItem<ParticularCustomItem>(7, "particular4", startItem);
			special5 = CreateOneItem<SpecialCustomItem>(8, "special5", startItem);
			other6 = CreateOneItem<OtherCustomItem>(9, "other6", startItem);
		}
	}
}