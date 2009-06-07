using System;
using System.Web.Mvc;
using N2.Engine;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Web.UI;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Extensions.Tests.Mvc.Html
{
	[TestFixture]
	public class EditableDisplayExtensionsTests
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			var templateRenderer = MockRepository.GenerateStub<ITemplateRenderer>();

			var engine = MockRepository.GenerateStub<IEngine>();
			engine.Expect(e => e.Resolve<ITemplateRenderer>()).Return(templateRenderer).Repeat.Any();

			Context.Initialize(engine);
		}

		#endregion

		public class RegularPageContainer : IItemContainer<RegularPage>
		{
			public RegularPageContainer(RegularPage page)
			{
				CurrentItem = page;
			}

			public RegularPage CurrentItem { get; set; }

			/// <summary>Gets the item associated with the item container.</summary>
			ContentItem IItemContainer.CurrentItem
			{
				get { return CurrentItem; }
			}
		}

		[Test]
		public void ViewPageSyntaxTestWithLambda()
		{
			var page = new N2ViewPage<RegularPage>
			           	{
			           		ViewData = new ViewDataDictionary<RegularPage>(new RegularPage
			           		                                               	{
			           		                                               		Title = "A Title",
			           		                                               	}),
			           		ViewContext = new ViewContext(),
			           	};
			page.InitHelpers();

			var result = page.EditableDisplay(m => m.Title).ToString();

			Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
		}

		[Test]
		public void ViewPageSyntaxTestWithString()
		{
			var page = new N2ViewPage<RegularPage>
			           	{
			           		ViewData = new ViewDataDictionary<RegularPage>(new RegularPage
			           		                                               	{
			           		                                               		Title = "A Title",
			           		                                               	}),
			           		ViewContext = new ViewContext(),
			           	};
			page.InitHelpers();

			var result = page.EditableDisplay("Title").ToString();

			Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
		}

		[Test]
		public void ModelViewPageSyntaxTestWithLambda()
		{
			var page = new N2ModelViewPage<RegularPageContainer, RegularPage>
			           	{
			           		ViewData = new ViewDataDictionary<RegularPageContainer>(new RegularPageContainer(new RegularPage
			           		                                                                                 	{
			           		                                                                                 		Title = "A Title",
			           		                                                                                 	})),
			           		ViewContext = new ViewContext(),
			           	};
			page.InitHelpers();

			var result = page.EditableDisplay(m => m.Title).ToString();

			Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
		}

		[Test]
		public void ModelViewPageSyntaxTestWithString()
		{
			var page = new N2ModelViewPage<RegularPageContainer, RegularPage>
			           	{
			           		ViewData = new ViewDataDictionary<RegularPageContainer>(new RegularPageContainer(new RegularPage
			           		                                                                                 	{
			           		                                                                                 		Title = "A Title",
			           		                                                                                 	})),
			           		ViewContext = new ViewContext(),
			           	};
			page.InitHelpers();

			var result = page.EditableDisplay("Title").ToString();

			Assert.That(result, Is.EqualTo("<h1>A Title</h1>"));
		}
	}
}