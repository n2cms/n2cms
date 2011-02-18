using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Principal;
using N2.Configuration;
using NUnit.Framework;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Persistence;
using N2.Tests.Definitions.Items;
using N2.Tests.Fakes;
using N2.Persistence.Proxying;
using N2.Security;
using Rhino.Mocks;
using N2.Web;
using N2.Definitions.Static;
using N2.Definitions.Runtime;
using N2.Web.Mvc.Html;
using N2.Web.UI;

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
			registration = new ContentRegistration();
			registration.ContentType = typeof(EmptyItem);
		}

		[Test]
		public void RegisteringCheckBox_AddsEditableCheckBox_ToDefinition()
		{
			registration.CheckBox("Visible", "Show the page in navigation");

			var definition = registration.CreateDefinition(new DefinitionTable());

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

			var definition = registration.CreateDefinition(new DefinitionTable());

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

			var definition = registration.CreateDefinition(new DefinitionTable());

			var editable = definition.Editables.Single();
			Assert.That(editable, Is.InstanceOf<WithEditableTitleAttribute>());
			Assert.That(editable.Name, Is.EqualTo("Title"));
			Assert.That(editable.Title, Is.EqualTo("The name of the page"));
		}

		[Test]
		public void Container()
		{
			registration.Tab("Content", "Primary content");

			var definition = registration.CreateDefinition(new DefinitionTable());

			var container = definition.Containers.Single();
			Assert.That(container, Is.InstanceOf<TabContainerAttribute>());
			Assert.That(container.Name, Is.EqualTo("Content"));
			Assert.That(((TabContainerAttribute)container).TabText, Is.EqualTo("Primary content"));
		}
	}
}