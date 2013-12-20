using System;
using System.Web.Mvc;
using System.Web.UI;
using N2.Details;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Web.UI;
using NUnit.Framework;

namespace N2.Extensions.Tests.Mvc.Html
{
    [TestFixture]
    public class DisplayExtensionsTests
    {
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
            public ActionResult Index()
            {
                return Content("Hello");
            }
        }

        [Test]
        public void ModelViewPageSyntaxTestWithLambda()
        {
            var item = new RegularPage { Title = "A Title" };
            var page = MvcTestUtilities.CreateContentViewPage(new RegularPageContainer(item), item);
            page.InitHelpers();

            var result = page.ContentHtml.DisplayContent(m => m.Title).ToString();

            Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
        }

        [Test]
        public void ModelViewPageSyntaxTestWithString()
        {
            var item = new RegularPage { Title = "A Title" };
            var page = MvcTestUtilities.CreateContentViewPage(new RegularPageContainer(item), item);
            page.InitHelpers();

            var result = page.ContentHtml.DisplayContent("Title").ToString();

            Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
        }

        [Test]
        public void ViewPageSyntaxTestWithLambda()
        {
            var page = MvcTestUtilities.CreateViewPage(new RegularPage { Title = "A Title" });
            page.InitHelpers();

            var result = page.Html.DisplayContent(m => m.Title).ToString();

            Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
        }

        [Test]
        public void ViewPageSyntaxTestWithString()
        {
            var page = MvcTestUtilities.CreateViewPage(new RegularPage { Title = "A Title" });
            page.InitHelpers();

            var result = page.Html.DisplayContent("Title").ToString();

            Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
        }

        //TODO: test sometime
        //[Test]
        //public void WhenPassedDisplayable_ForContentItem_PassesThatContentItem_AsModel()
        //{
        //    var testItem = new DisplayableItem
        //                    {
        //                        Text = "RootTestItem",
        //                        Property = new DisplayableItem {Text = "NestedTestItem"},
        //                    };
        //    var page = MvcTestUtilities.CreateViewPage(testItem);
        //    page.InitHelpers();

        //    var adapter = MockRepository.GenerateMock<MvcAdapter>();
        //    adapter.Expect(r => r.RenderTemplate(page.Html, testItem.Property));
            
        //    var adapterProvider = MockRepository.GenerateStub<IContentAdapterProvider>();
        //    adapterProvider.Expect(ap => ap.ResolveAdapter<MvcAdapter>(testItem.Property)).Return(adapter);

        //    var engine = page.ViewContext.RouteData.GetEngine();
        //    engine.Expect(e => e.Resolve<IContentAdapterProvider>()).Return(adapterProvider).Repeat.Any();

        //    var result = page.Html.DisplayContent(p => p.Property).ToString();

        //    adapter.VerifyAllExpectations();
        //}
    }
}
