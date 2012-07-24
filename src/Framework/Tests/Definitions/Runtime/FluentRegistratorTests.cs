using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions.Runtime;
using N2.Web.Mvc.Html;
using NUnit.Framework;
using Shouldly;
using N2.Details;

namespace N2.Tests.Definitions.Runtime
{
	[TestFixture]
	public class FluentRegistratorTests
	{
		private N2.Definitions.Static.DefinitionMap map;
		private FluentItemRegistration registration;

		class FluentItem : ContentItem
		{
			public virtual string Text { get; set; }
		}

		class FluentItemRegistration : FluentRegistrator<FluentItem>
		{
			public Action<ContentRegistration<FluentItem>> registerAction = delegate { };
			public override void RegisterDefinition(ContentRegistration<FluentItem> re)
			{
				registerAction(re);
			}
		}

		[SetUp]
		public void SetUp()
		{
			map = new N2.Definitions.Static.DefinitionMap();
			registration = new FluentItemRegistration();
		}

		[Test]
		public void Register_FreeText_ByExpression()
		{
			registration.registerAction = (re) => re.On(fi => fi.Text).FreeText();

			var definitions = registration.Register(map);

			var editable = definitions.Single().Editables.Single();
			editable.ShouldBeTypeOf<EditableFreeTextAreaAttribute>();
			editable.Name.ShouldBe("Text");
		}

		[Test]
		public void Register_FreeText_ByName()
		{
			registration.registerAction = (re) =>
			{
				re.On<string>("Text").FreeText();
			};

			var definitions = registration.Register(map);

			var editable = definitions.Single().Editables.Single();
			editable.ShouldBeTypeOf<EditableFreeTextAreaAttribute>();
			editable.Name.ShouldBe("Text");
		}

		[Test]
		public void Register_FreeText_AndConfigure()
		{
			registration.registerAction = (re) =>
			{
				re.On(fi => fi.Text).Text()
					.DefaultValue("hello")
					.Help("help title", "help body")
					.Required("This value is required")
					.RequiredPermission(N2.Security.Permission.Administer)
					.Container("ExtrasContainer")
					.Configure(ee => ee.Placeholder = "Some text in the box");
			};

			var definitions = registration.Register(map);

			var editable = (EditableTextAttribute)definitions.Single().Editables.Single();
			editable.Name.ShouldBe("Text");
			editable.DefaultValue.ShouldBe("hello");
			editable.HelpTitle.ShouldBe("help title");
			editable.HelpText.ShouldBe("help body");
			editable.Required.ShouldBe(true);
			editable.RequiredMessage.ShouldBe("This value is required");
			editable.RequiredPermission.ShouldBe(N2.Security.Permission.Administer);
			editable.ContainerName.ShouldBe("ExtrasContainer");
			editable.Placeholder.ShouldBe("Some text in the box");
		}
	}
}
