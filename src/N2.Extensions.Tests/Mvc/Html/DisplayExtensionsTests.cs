using System;
using System.Web.Mvc;
using System.Web.UI;
using N2.Details;
using N2.Engine;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Web.UI;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Extensions.Tests.Mvc.Html
{
	[TestFixture]
	public class DisplayExtensionsTests
	{
		private ITemplateRenderer _templateRenderer;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_templateRenderer = MockRepository.GenerateStub<ITemplateRenderer>();

			var engine = MockRepository.GenerateStub<IEngine>();
			engine.Expect(e => e.Resolve<ITemplateRenderer>()).Return(_templateRenderer).Repeat.Any();

			Context.Initialize(engine);
		}

		#endregion

		private class RegularPageContainer : IItemContainer<RegularPage>
		{
			public RegularPageContainer(RegularPage page)
			{
				CurrentItem = page;
			}

			#region IItemContainer<RegularPage> Members

			public RegularPage CurrentItem { get; set; }

			/// <summary>Gets the item associated with the item container.</summary>
			ContentItem IItemContainer.CurrentItem
			{
				get { return CurrentItem; }
			}

			#endregion
		}

		private class DisplayableItem : ContentItem
		{
			public string Text { get; set; }

			[TestDisplayable]
			public ContentItem Property
			{
				get { return GetDetail("Property") as ContentItem; }
				set { SetDetail("Property", value); }
			}

			public override string ToString()
			{
				return Text;
			}

			#region Nested type: TestDisplayableAttribute

			private class TestDisplayableAttribute : Attribute, IDisplayable
			{
				#region IDisplayable Members

				/// <summary>Gets or sets the name of the prpoerty referenced by this attribute.</summary>
				public string Name { get; set; }

				/// <summary>Creates, initializes adds and returns the displayer.</summary>
				/// <param name="item">The item from which to get it's value.</param>
				/// <param name="detailName"></param>
				/// <param name="container">The container onto which to add the displayer.</param>
				/// <returns>The displayer control that was added.</returns>
				public Control AddTo(ContentItem item, string detailName, Control container)
				{
					Name = detailName;

					var control = new LiteralControl("<displayed>" + item[detailName] + "</displayed>");

					container.Controls.Add(control);

					return control;
				}

				#endregion
			}

			#endregion
		}

		[Controls(typeof (DisplayableItem))]
		public class DisplayableItemController : Controller
		{
		}

		[Test]
		public void ModelViewPageSyntaxTestWithLambda()
		{
            var item = new RegularPage { Title = "A Title" };
            var page = MvcTestUtilities.CreateContentViewPage(new RegularPageContainer(item), item);
            page.InitHelpers();

            var result = page.ContentHtml.Display(m => m.Title).ToString();

			Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
		}

		[Test]
		public void ModelViewPageSyntaxTestWithString()
		{
            var item = new RegularPage { Title = "A Title" };
            var page = MvcTestUtilities.CreateContentViewPage(new RegularPageContainer(item), item);
			page.InitHelpers();

			var result = page.ContentHtml.Display("Title").ToString();

			Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
		}

		[Test]
		public void ViewPageSyntaxTestWithLambda()
		{
            var page = MvcTestUtilities.CreateViewPage(new RegularPage { Title = "A Title" });
			page.InitHelpers();

			var result = page.Html.Display(m => m.Title).ToString();

			Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
		}

		[Test]
		public void ViewPageSyntaxTestWithString()
		{
            var page = MvcTestUtilities.CreateViewPage(new RegularPage { Title = "A Title" });
			page.InitHelpers();

            var result = page.Html.Display("Title").ToString();

			Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
		}

		[Test]
		public void WhenPassedDisplayableForContentItemPassesThatContentItemAsModel()
		{
			_templateRenderer.Expect(r => r.RenderTemplate(null, null))
				.Return("Testing")
				.IgnoreArguments().Repeat.Once();

			var testItem = new DisplayableItem
			               	{
			               		Text = "RootTestItem",
			               		Property = new DisplayableItem {Text = "NestedTestItem"},
			               	};
            var page = MvcTestUtilities.CreateViewPage(testItem);
			page.InitHelpers();

            var result = page.Html.Display(p => p.Property).ToString();

			Assert.That(result, Is.EqualTo("Testing"));
			_templateRenderer.VerifyAllExpectations();
		}
	}
}