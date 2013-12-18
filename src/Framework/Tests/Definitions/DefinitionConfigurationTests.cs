using System;
using System.Linq;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Details;
using N2.Tests.Definitions.Items;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Definitions
{
    [TestFixture]
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
            var definitionCollection = new DefinitionCollection { DefineUnattributedTypes = true };
            definitionCollection.Add(new DefinitionElement { Name = "DefinitionUndefined", Type = typeof(DefinitionUndefined).AssemblyQualifiedName });
            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], new EngineSection { Definitions = definitionCollection });

            var definitions = builder.GetDefinitions();
            var undefinedDefinition = definitions
                .Single(d => d.ItemType == typeof(DefinitionUndefined));

            Assert.That(undefinedDefinition.IsDefined, Is.True);
        }

        [Test]
        public void Configuration_CanRemove_Definition()
        {
            var definitionCollection = new DefinitionCollection { DefineUnattributedTypes = true };
            definitionCollection.Remove(new DefinitionElement { Name = "DefinitionTextPage" });
            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], new EngineSection { Definitions = definitionCollection });

            var definitions = builder.GetDefinitions();
            var textPageDefinitions = definitions
                .Where(d => d.ItemType == typeof(DefinitionTextPage));

            Assert.That(textPageDefinitions.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Configuration_CanAdd_Editable_ToDefinition()
        {
            DefinitionElement definitionElement = new DefinitionElement { Name = "DefinitionTextPage" };
            definitionElement.Containers.Add(new ContainableElement { Name = "X", Type = typeof(EditableCheckBoxAttribute).AssemblyQualifiedName });
            var definitionCollection = new DefinitionCollection { DefineUnattributedTypes = true };
            definitionCollection.Add(definitionElement);

            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], new EngineSection { Definitions = definitionCollection });

            var definitions = builder.GetDefinitions();
            var textPageDefinition = definitions
                .Single(d => d.ItemType == typeof(DefinitionTextPage));

            var textEditors = textPageDefinition.Editables
                .Where(e => e.GetType() == typeof(EditableCheckBoxAttribute));

            Assert.That(textEditors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Configuration_CanAdd_Editable_ToExistingDefinition()
        {
            var definitionCollection = new DefinitionCollection { DefineUnattributedTypes = true };
            definitionCollection.Add(new DefinitionElement
            {
                Name = "DefinitionTextPage",
                Editables = new ContainableCollection { new ContainableElement {
                    Name = "MetaTitle", 
                    Title = "Page title", 
                    Type = typeof(N2.Details.EditableTextAttribute).AssemblyQualifiedName } }
            });
            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], new EngineSection { Definitions = definitionCollection });

            var definitions = builder.GetDefinitions();
            var textDefinition = definitions.Single(d => d.ItemType == typeof(DefinitionTextPage));

            Assert.That(textDefinition.Editables.Any(e => e.Name == "MetaTitle"));
        }

        [Test]
        public void Configuration_CanChange_Editable_OnExistingDefinition()
        {
            var definitionCollection = new DefinitionCollection { DefineUnattributedTypes = true };
            definitionCollection.Add(new DefinitionElement
            {
                Name = "DefinitionTextPage",
                Editables = new ContainableCollection { new ContainableElement {
                    Name = "Title", 
                    Title = "Page title in navigation", 
                    Type = typeof(EditableTextAttribute).AssemblyQualifiedName } }
            });
            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], new EngineSection { Definitions = definitionCollection });

            var definitions = builder.GetDefinitions();
            var textDefinition = definitions.Single(d => d.ItemType == typeof(DefinitionTextPage));

            Assert.That(textDefinition.Editables.Any(e => e.Title == "Page title in navigation" && e.GetType() == typeof(EditableTextAttribute)));
            Assert.That(textDefinition.Editables.Any(e => e.Title == "Title" || e.GetType() == typeof(WithEditableTitleAttribute)), Is.False);
        }

        [Test]
        public void Configuration_CanRemove_Editable_FromDefinition()
        {
            DefinitionElement definitionElement = new DefinitionElement { Name = "DefinitionTextPage" };
            definitionElement.Containers.Remove(new ContainableElement { Name = "Text" });
            var definitionCollection = new DefinitionCollection { DefineUnattributedTypes = true };
            definitionCollection.Add(definitionElement);

            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], new EngineSection { Definitions = definitionCollection });

            var definitions = builder.GetDefinitions();
            var textPageDefinition = definitions
                .Single(d => d.ItemType == typeof(DefinitionTextPage));

            var textEditors = textPageDefinition.Editables
                .Where(e => e.GetType() == typeof(EditableFreeTextAreaAttribute));

            Assert.That(textEditors.Count(), Is.EqualTo(0));
        }
    }
}
