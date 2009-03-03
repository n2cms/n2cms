using N2.Configuration;
using N2.Persistence;
using N2.Tests.Fakes;
using NUnit.Framework;
using N2.Web;
using NUnit.Framework.SyntaxHelpers;
using N2.Tests.Web.Items;
using N2.Engine;

namespace N2.Tests.Web
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
			HostSection hostSection = new HostSection {Web = new WebElement {ObserveEmptyExtension = true}};
			parser = new UrlParser(persister, webContext, new ItemNotifier(), new Host(webContext, startItem.ID, startItem.ID), hostSection);
			dispatcher = new RequestDispatcher(null, webContext, parser, new AppDomainTypeFinder(), new ErrorHandler(webContext, null, null), hostSection);
			dispatcher.Start();
		}

		[Test]
		public void CanResolve_DefaultRequestController()
		{
			SetUrl("/");

			RequestAspectController controller = dispatcher.ResolveAspectController<RequestAspectController>();

			Assert.That(controller, Is.TypeOf(typeof(RequestAspectController)));
		}

		[Test]
		public void CanResolve_CustomController()
		{
			SetUrl("/custom3.aspx");

			RequestAspectController controller = dispatcher.ResolveAspectController<RequestAspectController>();

			Assert.That(controller, Is.TypeOf(typeof(CustomRequestController)));
		}

		[Test]
		public void CanResolve_CustomController_OnInheritedItem()
		{
			SetUrl("/particular4.aspx");

			RequestAspectController controller = dispatcher.ResolveAspectController<RequestAspectController>();

			Assert.That(controller, Is.TypeOf(typeof(CustomRequestController)));
		}

		[Test]
		public void CanResolve_CustomController_WithRedefinedController()
		{
			SetUrl("/special5.aspx");

			RequestAspectController controller = dispatcher.ResolveAspectController<RequestAspectController>();

			Assert.That(controller, Is.TypeOf(typeof(SpecialCustomController)));
		}

		[Test]
		public void CanResolve_CustomController_WithOtherController()
		{
			SetUrl("/other6.aspx");

			RequestAspectController controller = dispatcher.ResolveAspectController<RequestAspectController>();

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

		void SetUrl(string url)
		{
			webContext.Url = url;
			webContext.CurrentPath = parser.ResolvePath(url);
		}
	}
}