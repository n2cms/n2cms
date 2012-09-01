using System.Linq;
using N2.Definitions;
using N2.Definitions.Runtime;
using N2.Definitions.Static;
using N2.Details;
using N2.Web.Mvc.Html;
using N2.Web.UI;
using NUnit.Framework;
using N2.Integrity;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class ContentRegistrationTests
	{
		ContentRegistration registration;
		
		class EmptyItem : ContentItem
		{
		}

		[SetUp]
		public void SetUp()
		{
			registration = new ContentRegistration(new DefinitionMap().GetOrCreateDefinition(typeof(EmptyItem)));
		}

		[Test]
		public void RegisteringCheckBox_AddsEditableCheckBox_ToDefinition()
		{
			registration.CheckBox("Visible", "Show the page in navigation");

			var definition = registration.Finalize();

			var editable = (EditableCheckBoxAttribute)definition.Editables.Single();
			Assert.That(editable, Is.InstanceOf<EditableCheckBoxAttribute>());
			Assert.That(editable.Name, Is.EqualTo("Visible"));
			Assert.That(editable.Title, Is.EqualTo(""));
			Assert.That(editable.CheckBoxText, Is.EqualTo("Show the page in navigation"));
		}

		[Test]
		public void RegisteringDateRange_AddsEditableDateRange_ToDefinition()
		{
			registration.DateRange("From", "To", "Opening hours");

			var definition = registration.Finalize();

			var editable = (WithEditableDateRangeAttribute)definition.Editables.Single();
			Assert.That(editable, Is.InstanceOf<WithEditableDateRangeAttribute>());
			Assert.That(editable.Name, Is.EqualTo("From"));
			Assert.That(editable.NameEndRange, Is.EqualTo("To"));
			Assert.That(editable.Title, Is.EqualTo("Opening hours"));
		}

		[Test]
		public void RegisteringTitle_AddsEditableTitle_ToDefinition()
		{
			registration.Title("The name of the page");

			var definition = registration.Finalize();

			var editable = definition.Editables.Single();
			Assert.That(editable, Is.InstanceOf<WithEditableTitleAttribute>());
			Assert.That(editable.Name, Is.EqualTo("Title"));
			Assert.That(editable.Title, Is.EqualTo("The name of the page"));
		}

		[Test]
		public void Container_CanBeRegistered()
		{
			registration.Tab("Content", "Primary content");

			var definition = registration.Finalize();

			var container = definition.Containers.Single();
			Assert.That(container, Is.InstanceOf<TabContainerAttribute>());
			Assert.That(container.Name, Is.EqualTo("Content"));
			Assert.That(((TabContainerAttribute)container).TabText, Is.EqualTo("Primary content"));
		}

		[Test]
		public void Container_WithEditable_CanBeRegistered()
		{
			registration.Tab("Content", "Primary content", re => re.FreeText("Text"));

			var definition = registration.Finalize();

			var editable = definition.Editables.Single();
			Assert.That(editable.ContainerName, Is.EqualTo("Content"));
		}

		[Test]
		public void Displayable_CanBeRegistered()
		{
			registration.Add(new DisplayableTokensAttribute { Name = "Hello" });

			var definition = registration.Finalize();

			var displayable = definition.Displayables.Single(d => d.Name == "Hello");
			Assert.That(displayable, Is.InstanceOf<DisplayableTokensAttribute>());
			Assert.That(displayable.Name, Is.EqualTo("Hello"));
		}

		[Test]
		public void EditableAndDisplayable_SharingTheSameTurf()
		{
			registration.FreeText("Hello");
			registration.Add(new DisplayableTokensAttribute { Name = "Hello" });

			var definition = registration.Finalize();

			var editable = definition.Editables.Single(d => d.Name == "Hello");
			var displayable = definition.Displayables.Single(d => d.Name == "Hello");

			Assert.That(editable, Is.InstanceOf<EditableFreeTextAreaAttribute>());
			Assert.That(editable.Name, Is.EqualTo("Hello"));
			Assert.That(displayable, Is.InstanceOf<DisplayableTokensAttribute>());
			Assert.That(displayable.Name, Is.EqualTo("Hello"));
		}

		[Test]
		public void Restrictions_CanBeRegistered()
		{
			registration.RestrictChildren("Hello");
			registration.IsDefined = true;

			var definition = registration.Finalize();

			Assert.That(definition.AllowedChildFilters.OfType<RestrictChildrenAttribute>().Single().TemplateNames.Single(), Is.EqualTo("Hello"));
		}

		[Test]
		public void Restrictions_CanBeRegistered_AndConfigured()
		{
			registration.RestrictChildren(typeof(Definitions.SideshowItem)).Configure(rc => rc.TemplateNames = new [] { "World" });
			registration.IsDefined = true;

			var definition = registration.Finalize();

			Assert.That(definition.AllowedChildFilters.OfType<RestrictChildrenAttribute>().Single().Types.Single(), Is.EqualTo(typeof(Definitions.SideshowItem)));
			Assert.That(definition.AllowedChildFilters.OfType<RestrictChildrenAttribute>().Single().TemplateNames.Single(), Is.EqualTo("World"));
		}
	}
}