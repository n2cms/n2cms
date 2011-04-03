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
using N2.Definitions.Static;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class SortByTests
	{
		ContentItem parent;

		[SetUp]
		public void SetUp()
		{
			parent = new DefinitionOne();
			new DefinitionOne { SortOrder = 1, Name = "c", Published = new DateTime(2010, 11, 10), Title = "b", Updated = new DateTime(2011, 01, 10) }.AddTo(parent);
			new DefinitionOne { SortOrder = 2, Name = "a", Published = new DateTime(2010, 10, 10), Title = "A", Updated = new DateTime(2010, 10, 10) }.AddTo(parent);
			new DefinitionOne { SortOrder = 0, Name = "b", Published = new DateTime(2010, 12, 10), Title = "C", Updated = new DateTime(2010, 12, 10) }.AddTo(parent);
		}

		[Test]
		public void SortBy_CurrentOrder()
		{
			new SortChildrenAttribute(SortBy.CurrentOrder).ReorderChildren(parent);

			Assert.That(parent.Children[0].SortOrder, Is.EqualTo(1));
			Assert.That(parent.Children[1].SortOrder, Is.EqualTo(2));
			Assert.That(parent.Children[2].SortOrder, Is.EqualTo(3));
		}

		[Test]
		public void SortBy_Expression()
		{
			new SortChildrenAttribute(SortBy.Expression) { SortExpression = "Name" }.ReorderChildren(parent);

			Assert.That(parent.Children[0].Name, Is.EqualTo("a"));
			Assert.That(parent.Children[1].Name, Is.EqualTo("b"));
			Assert.That(parent.Children[2].Name, Is.EqualTo("c"));
		}

		[Test]
		public void SortBy_Expression_DESC()
		{
			new SortChildrenAttribute(SortBy.Expression) { SortExpression = "Name DESC" }.ReorderChildren(parent);

			Assert.That(parent.Children[0].Name, Is.EqualTo("c"));
			Assert.That(parent.Children[1].Name, Is.EqualTo("b"));
			Assert.That(parent.Children[2].Name, Is.EqualTo("a"));
		}

		[Test]
		public void SortBy_Published()
		{
			new SortChildrenAttribute(SortBy.Published).ReorderChildren(parent);

			Assert.That(parent.Children[0].Published, Is.EqualTo(new DateTime(2010, 10, 10)));
			Assert.That(parent.Children[1].Published, Is.EqualTo(new DateTime(2010, 11, 10)));
			Assert.That(parent.Children[2].Published, Is.EqualTo(new DateTime(2010, 12, 10)));
		}

		[Test]
		public void SortBy_PublishedDescending()
		{
			new SortChildrenAttribute(SortBy.PublishedDescending).ReorderChildren(parent);

			Assert.That(parent.Children[0].Published, Is.EqualTo(new DateTime(2010, 12, 10)));
			Assert.That(parent.Children[1].Published, Is.EqualTo(new DateTime(2010, 11, 10)));
			Assert.That(parent.Children[2].Published, Is.EqualTo(new DateTime(2010, 10, 10)));
		}

		[Test]
		public void SortBy_Title()
		{
			new SortChildrenAttribute(SortBy.Title).ReorderChildren(parent);

			Assert.That(parent.Children[0].Title, Is.EqualTo("A"));
			Assert.That(parent.Children[1].Title, Is.EqualTo("b"));
			Assert.That(parent.Children[2].Title, Is.EqualTo("C"));
		}

		[Test]
		public void SortBy_Unordered()
		{
			new SortChildrenAttribute(SortBy.Unordered).ReorderChildren(parent);

			Assert.That(parent.Children[0].Name, Is.EqualTo("c"));
			Assert.That(parent.Children[1].Name, Is.EqualTo("a"));
			Assert.That(parent.Children[2].Name, Is.EqualTo("b"));
		}

		[Test]
		public void SortBy_Updated()
		{
			new SortChildrenAttribute(SortBy.Updated).ReorderChildren(parent);

			Assert.That(parent.Children[0].Updated, Is.EqualTo(new DateTime(2010, 10, 10)));
			Assert.That(parent.Children[1].Updated, Is.EqualTo(new DateTime(2010, 12, 10)));
			Assert.That(parent.Children[2].Updated, Is.EqualTo(new DateTime(2011, 01, 10)));
		}

		[Test]
		public void SortBy_UpdatedDescending()
		{
			new SortChildrenAttribute(SortBy.UpdatedDescending).ReorderChildren(parent);

			Assert.That(parent.Children[0].Updated, Is.EqualTo(new DateTime(2011, 01, 10)));
			Assert.That(parent.Children[1].Updated, Is.EqualTo(new DateTime(2010, 12, 10)));
			Assert.That(parent.Children[2].Updated, Is.EqualTo(new DateTime(2010, 10, 10)));
		}
	}

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
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection });

			var definitions = builder.GetDefinitions();
			var undefinedDefinition = definitions
				.Single(d => d.ItemType == typeof(DefinitionUndefined));

			Assert.That(undefinedDefinition.IsDefined, Is.True);
		}

		[Test]
		public void Configuration_CanRemove_Definition()
		{
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Remove(new DefinitionElement { Name = "DefinitionTextPage" });
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection });

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
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Add(definitionElement);

			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection });

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
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Add(new DefinitionElement
			{
				Name = "DefinitionTextPage",
				Editables = new ContainableCollection { new ContainableElement {
					Name = "MetaTitle", 
					Title = "Page title", 
					Type = typeof(N2.Details.EditableTextAttribute).AssemblyQualifiedName } }
			});
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection });

			var definitions = builder.GetDefinitions();
			var textDefinition = definitions.Single(d => d.ItemType == typeof(DefinitionTextPage));

			Assert.That(textDefinition.Editables.Any(e => e.Name == "MetaTitle"));
		}

		[Test]
		public void Configuration_CanChange_Editable_OnExistingDefinition()
		{
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Add(new DefinitionElement
			{
				Name = "DefinitionTextPage",
				Editables = new ContainableCollection { new ContainableElement {
					Name = "Title", 
					Title = "Page title in navigation", 
					Type = typeof(EditableTextAttribute).AssemblyQualifiedName } }
			});
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection });

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
			var definitionCollection = new DefinitionCollection();
			definitionCollection.Add(definitionElement);

			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection { Definitions = definitionCollection });

			var definitions = builder.GetDefinitions();
			var textPageDefinition = definitions
				.Single(d => d.ItemType == typeof(DefinitionTextPage));

			var textEditors = textPageDefinition.Editables
				.Where(e => e.GetType() == typeof(EditableFreeTextAreaAttribute));

			Assert.That(textEditors.Count(), Is.EqualTo(0));
		}
	}
}
