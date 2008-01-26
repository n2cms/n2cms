using System;
using System.Collections.Generic;
using System.Text;

using N2;
using MbUnit.Framework;
using N2.Definitions;

namespace N2.Tests.Integrity
{
	[TestFixture, TestCategory("Integration")]
	public class IntegrityFixture : Persistence.DatabasePreparingBase
	{
		[Test]
		public void AllowedItemBelowRoot()
		{
			Definitions.StartPage root = CreateRoot();
			ContentItem item = CreateItemBelow(root, typeof(Definitions.Page));
			Assert.IsNotNull(item);
			engine.Persister.Delete(root);
		}

		[Test, ExpectedException(typeof(N2.Definitions.NotAllowedParentException))]
		public void UnAllowedItemBelowRoot()
		{
			Definitions.StartPage root = CreateRoot();
			try
			{
				CreateItemBelow(root, typeof(Definitions.SubPage));
			}
			finally
			{
				engine.Persister.Delete(root);
			}
		}

		[Test]
		public void AllowedPropagatesToSubclasses()
		{
			ContentItem root = CreateItemBelow(null, typeof(Definitions.AlternativeStartPage));
			ContentItem item = CreateItemBelow(root, typeof(Definitions.Page));
			Assert.IsNotNull(item);
			engine.Persister.Delete(root);
		}

		[Test]
		public void AllowedItemBelowSubClassOfRoot()
		{
			ContentItem root = CreateItemBelow(null, typeof(Definitions.AlternativeStartPage));
			ContentItem item = CreateItemBelow(root, typeof(Definitions.AlternativePage));
			Assert.IsNotNull(item);
			engine.Persister.Delete(root);
		}

		[Test, ExpectedException(typeof(N2.Definitions.NotAllowedParentException))]
		public void AllowedItemBelowSubClassOfRootNotAllowedBelowRootItem()
		{
			ContentItem root = CreateRoot();
			try
			{
				CreateItemBelow(root, typeof(Definitions.AlternativePage));
			}
			finally
			{
				engine.Persister.Delete(root);
			}
		}

		[Test]
		public void ValidateRootDefinition()
		{
			ItemDefinition definition = engine.Definitions.GetDefinition(typeof(Definitions.Root));
			ItemDefinition startPageDefinition = engine.Definitions.GetDefinition(typeof(Definitions.StartPage));
			EnumerableAssert.Contains(definition.AllowedChildren, startPageDefinition);
			//Assert.AreEqual(1, definition.AllowedZoneNames.Count);
			Assert.IsNull(definition.AuthorizedRoles);
			Assert.AreEqual(0, definition.AvailableZones.Count);
			Assert.AreEqual(0, definition.Containers.Count);
			Assert.AreEqual("", definition.Description);
			Assert.AreEqual(typeof(Definitions.Root).Name, definition.Discriminator);
			Assert.AreEqual(1, definition.Displayables.Count);
			Assert.AreEqual(0, definition.Editables.Count);
			EnumerableAssert.Contains(definition.GetAllowedChildren(null, null), startPageDefinition);
			Assert.AreEqual(0, definition.GetEditables(null).Count);
			Assert.AreEqual(0, definition.GetModifiers("Title").Count); 
			Assert.AreEqual(0, definition.Modifiers.Count);
			Assert.AreEqual(1000, definition.SortOrder);
			Assert.AreEqual(typeof(Definitions.Root).Name, definition.Title);
			Assert.AreEqual(typeof(Definitions.Root).FullName, definition.ToolTip);
		}

		[Test]
		public void AllowedChildren_Superceedes_NoAllowedParents()
		{
			ContentItem realRoot = CreateItemBelow(null, typeof(Definitions.Root));
			ContentItem item = CreateItemBelow(realRoot, typeof(Definitions.StartPage));
			Assert.IsNotNull(item);
			engine.Persister.Delete(realRoot);
		}

		[Test, ExpectedException(typeof(N2.Definitions.NotAllowedParentException))]
		public void RootIsntAllowedBelowAllowedItemBelowRoot()
		{
			ContentItem root = CreateItemBelow(null, typeof(Definitions.Page));
			try
			{
				CreateItemBelow(root, typeof(Definitions.StartPage));
			}
			finally
			{
				engine.Persister.Delete(root);
			}
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
