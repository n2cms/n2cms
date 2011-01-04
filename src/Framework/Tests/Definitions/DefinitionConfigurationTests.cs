using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Configuration;
using N2.Definitions;
using N2.Tests.Definitions.Items;
using N2.Tests.Fakes;
using NUnit.Framework;
using N2.Details;

namespace N2.Tests.Definitions
{
	public class DefinitionConfigurationTests : TypeFindingBase
	{
		protected override Type[] GetTypes()
		{
			return new[]
			{
				typeof (DefinitionStartPage),
				typeof (DefinitionTextPage),
				typeof (DefinitionUndefined)
			};
		}



		[Test]
		public void Configuration_CanAdd_Definition()
		{
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Add(new DefinitionElement { Name = "DefinitionUndefined", Type = typeof(DefinitionUndefined).AssemblyQualifiedName });
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection }, new FakeEditUrlManager());

			var definitions = builder.GetDefinitions();
			var undefinedDefinition = definitions.Values
				.Where(d => d.ItemType == typeof(DefinitionUndefined))
				.Single();

			Assert.That(undefinedDefinition.IsDefined, Is.True);
		}

		[Test]
		public void Configuration_CanRemove_Definition()
		{
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Remove(new DefinitionElement { Name = "DefinitionTextPage" });
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection }, new FakeEditUrlManager());

			var definitions = builder.GetDefinitions();
			var textPageDefinitions = definitions.Values
				.Where(d => d.ItemType == typeof(DefinitionTextPage));

			Assert.That(textPageDefinitions.Count(), Is.EqualTo(0));
		}

		[Test]
		public void Configuration_CanAdd_Editable_ToDefinition()
		{
			DefinitionElement definitionElement = new DefinitionElement { Name = "DefinitionTextPage" };
			definitionElement.Containers.Add(new ContainableElement { Name = "X", Type = typeof(EditableCheckBoxAttribute).AssemblyQualifiedName });
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Add(definitionElement);

			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection }, new FakeEditUrlManager());

			var definitions = builder.GetDefinitions();
			var textPageDefinition = definitions.Values
				.Where(d => d.ItemType == typeof(DefinitionTextPage))
				.Single();

			var textEditors = textPageDefinition.Editables
				.Where(e => e.GetType() == typeof(EditableCheckBoxAttribute));

			Assert.That(textEditors.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Configuration_CanRemove_Editable_FromDefinition()
		{
			DefinitionElement definitionElement = new DefinitionElement { Name = "DefinitionTextPage" };
			definitionElement.Containers.Remove(new ContainableElement { Name = "Text" });
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Add(definitionElement);

			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection }, new FakeEditUrlManager());

			var definitions = builder.GetDefinitions();
			var textPageDefinition = definitions.Values
				.Where(d => d.ItemType == typeof(DefinitionTextPage))
				.Single();

			var textEditors = textPageDefinition.Editables
				.Where(e => e.GetType() == typeof(EditableFreeTextAreaAttribute));

			Assert.That(textEditors.Count(), Is.EqualTo(0));
		}
	}
}
