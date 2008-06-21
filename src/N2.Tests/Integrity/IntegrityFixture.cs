using System;
using System.Collections.Generic;
using System.Text;

using N2;
using NUnit.Framework;
using N2.Definitions;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Integrity
{
	[TestFixture]
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
			ItemDefinition rootDefinition = engine.Definitions.GetDefinition(typeof(Definitions.Root));
			ItemDefinition startPageDefinition = engine.Definitions.GetDefinition(typeof(Definitions.StartPage));
			
			EnumerableAssert.Contains(rootDefinition.AllowedChildren, startPageDefinition);
			Assert.IsNull(rootDefinition.AuthorizedRoles);
			Assert.AreEqual(0, rootDefinition.AvailableZones.Count);
			Assert.AreEqual(0, rootDefinition.Containers.Count);
			Assert.IsEmpty(rootDefinition.Description);
			Assert.AreEqual(typeof(Definitions.Root).Name, rootDefinition.Discriminator);
			Assert.That(rootDefinition.Displayables.Count, Is.EqualTo(9));
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
			engine.Persister.Delete(realRoot);
		}

		[Test]
		public void Root_IsntAllowed_BelowAllowedItem_BelowRoot()
		{
			ContentItem root = CreateItemBelow(null, typeof(Definitions.Page));

			ExceptionAssert.Throws<NotAllowedParentException>(delegate
			{
				CreateItemBelow(root, typeof(Definitions.StartPage));
			});
		
			engine.Persister.Delete(root);
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
