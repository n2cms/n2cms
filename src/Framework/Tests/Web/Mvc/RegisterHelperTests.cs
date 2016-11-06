using N2.Definitions;
using N2.Definitions.Runtime;
using N2.Details;
using N2.Web;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace N2.Tests.Web.Mvc
{
	public class RegisterHelperTestsItem : ContentItem
	{
		[EditableMediaUpload]
		public string EditableMediaUpload { get; set; }
	}

	[TestFixture]
	public class RegisterHelperTests
	{
		private ItemDefinition definition;
		private RegisterHelper helper;

		[SetUp]
		public void SetUp()
		{
			var item = new RegisterHelperTestsItem();
			definition = new ItemDefinition(item.GetType());
			var re = new ContentRegistration(definition);
			
			var html = new HtmlHelper(new ViewContext { HttpContext = new Fakes.FakeHttpContext("/") }, MockRepository.GenerateStub<IViewDataContainer>());
			RegistrationExtensions.SetRegistrationExpression(html.ViewContext.HttpContext, re);
			helper = new RegisterHelper(html);
		}

		[Test]
		public void RegisteredImage_ShouldBePresentInDefinition()
		{
			helper.Image("NewImage");

			definition.Editables.Single(e => e.Name == "NewImage").GetType().ShouldBe(typeof(EditableImageAttribute));
			definition.Properties["NewImage"].Editable.GetType().ShouldBe(typeof(EditableImageAttribute));
		}

		[Test]
		public void RedefineExistingProperty_SholudReplacePreviousProperty()
		{
			helper.Image("EditableMediaUpload");

			definition.Editables.Single(e => e.Name == "EditableMediaUpload").GetType().ShouldBe(typeof(EditableImageAttribute));
			definition.Properties["EditableMediaUpload"].Editable.GetType().ShouldBe(typeof(EditableImageAttribute));
		}
	}
}
