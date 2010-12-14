using System;
using NUnit.Framework;
using N2.Definitions;

namespace N2.Tests.Integrity
{
	[TestFixture, Category("Integration")]
	public class IntegrityFixture : Persistence.DatabasePreparingBase
	{
		[Test]
		public void AllowedItemBelowRoot()
		{
			Definitions.StartPage root = CreateRoot();
			ContentItem item = CreateItemBelow(root, typeof(Definitions.Page));

			Assert.IsNotNull(item);
		}

		[Test]
		public void AllowedPropagatesToSubclasses()
		{
			ContentItem root = CreateItemBelow(null, typeof(Definitions.AlternativeStartPage));
			ContentItem item = CreateItemBelow(root, typeof(Definitions.Page));

			Assert.IsNotNull(item);
		}

		[Test]
		public void AllowedItemBelowSubClassOfRoot()
		{
			ContentItem root = CreateItemBelow(null, typeof(Definitions.AlternativeStartPage));
			ContentItem item = CreateItemBelow(root, typeof(Definitions.AlternativePage));

			Assert.IsNotNull(item);
		}

		[Test]
		public void ValidateRootDefinition()
		{
			ItemDefinition rootDefinition = engine.Definitions.GetDefinition(typeof(Definitions.Root));
			ItemDefinition startPageDefinition = engine.Definitions.GetDefinition(typeof(Definitions.StartPage));
			
			EnumerableAssert.Contains(rootDefinition.AllowedChildren, startPageDefinition);
			Assert.IsNull(rootDefinition.AuthorizedRoles);
			Assert.AreEqual(0, rootDefinition.AvailableZones.Count);
			Assert.AreEqual(0, rootDefinition.Containers.Count);
			Assert.IsEmpty(rootDefinition.Description);
			Assert.AreEqual(typeof(Definitions.Root).Name, rootDefinition.Discriminator);
			Assert.That(rootDefinition.Displayables.Count, Is.EqualTo(12));
			Assert.AreEqual(0, rootDefinition.Editables.Count);
			EnumerableAssert.Contains(engine.Definitions.GetAllowedChildren(rootDefinition, null, null), startPageDefinition);
			Assert.AreEqual(0, rootDefinition.GetEditables(null).Count);
			Assert.AreEqual(0, rootDefinition.GetModifiers("Title").Count); 
			Assert.AreEqual(0, rootDefinition.Modifiers.Count);
			Assert.AreEqual(0, rootDefinition.SortOrder);
			Assert.AreEqual(typeof(Definitions.Root).Name, rootDefinition.Title);
		}

		[Test]
		public void AllowedChildren_Superceedes_NoAllowedParents()
		{
			ContentItem realRoot = CreateItemBelow(null, typeof(Definitions.Root));
			ContentItem item = CreateItemBelow(realRoot, typeof(Definitions.StartPage));
			
			Assert.IsNotNull(item);
		}

		private Definitions.StartPage CreateRoot()
		{
			return (Definitions.StartPage)CreateItemBelow(null, typeof(Definitions.StartPage));
		}

		private ContentItem CreateItemBelow(ContentItem parent, Type itemType)
		{
			ContentItem item = engine.Definitions.CreateInstance(itemType, parent);
			engine.Persister.Save(item);
			return item;
		}
	}
}
