using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Configuration;
using N2.Definitions;
using N2.Tests.Definitions.Items;
using NUnit.Framework;

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
		public void Can_AddDefinition_UsingConfiguration()
		{
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Add(new DefinitionElement { Name = "DefinitionUndefined", Type = typeof(DefinitionUndefined).AssemblyQualifiedName });
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection });

			var definitions = builder.GetDefinitions();
			var undefinedDefinition = definitions.Values
				.Where(d => d.ItemType == typeof (DefinitionUndefined))
				.Single();

			Assert.That(undefinedDefinition.IsDefined, Is.True);
		}

		[Test]
		public void Can_RemoveDefinition_UsingConfiguration()
		{
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Remove(new DefinitionElement { Name = "DefinitionTextPage", Type = typeof(DefinitionTextPage).AssemblyQualifiedName });
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection });

			var definitions = builder.GetDefinitions();
			var textPageDefinition = definitions.Values
				.Where(d => d.ItemType == typeof (DefinitionTextPage))
				.Single();

			Assert.That(textPageDefinition.IsDefined, Is.False);
		}
	}
}
