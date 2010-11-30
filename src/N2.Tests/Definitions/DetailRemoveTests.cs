using System;
using System.Linq;
using N2.Configuration;
using N2.Definitions;
using N2.Tests.Definitions.Items;
using N2.Tests.Fakes;
using NUnit.Framework;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class DetailRemoveTests : TypeFindingBase
	{
		DefinitionBuilder builder;
		protected override Type[] GetTypes()
		{
			return new Type[]
				{
					typeof (DefinitionRemovable),
					typeof (DefinitionRemoves)
				};
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			builder = new DefinitionBuilder(typeFinder, new EngineSection(), new FakeEditUrlManager());
		}

		[Test]
		public void CanRemove_Detail_InheritedFrom_BaseClass()
		{
			var definitions = builder.GetDefinitions();

			var descriptionEditables = definitions[typeof (DefinitionRemoves)]
				.Editables.Where(e => e.Name == "Description");

			Assert.That(descriptionEditables.Count(), Is.EqualTo(0));
		}


		[Test]
		public void RemoveDetail_OnInheritedClass_DoesntAffectBaseClass()
		{
			var definitions = builder.GetDefinitions();

			var descriptionEditables = definitions[typeof(DefinitionRemovable)]
				.Editables.Where(e => e.Name == "Description");

			Assert.That(descriptionEditables.Count(), Is.EqualTo(1));
		}

		[Test]
		public void CanRemove_Detail_UsingAssemblyAttribute()
		{
			var definitions = builder.GetDefinitions();

			var textEditables = definitions[typeof(DefinitionRemovable)]
				.Editables.Where(e => e.Name == "Text");

			Assert.That(textEditables.Count(), Is.EqualTo(0));
		}

		[Test]
		public void RemoveDetail_UsingAssemblyAttribute_AffectsDerivedClasses()
		{
			var definitions = builder.GetDefinitions();

			var textEditables = definitions[typeof(DefinitionRemoves)]
				.Editables.Where(e => e.Name == "Text");

			Assert.That(textEditables.Count(), Is.EqualTo(0));
		}
	}
}
