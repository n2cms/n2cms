using System;
using System.Linq;
using N2.Definitions;
using N2.Persistence;
using NUnit.Framework;

namespace N2.Tests.Integrity
{
    [TestFixture, Category("Integration")]
    public class IntegrityFixture : Persistence.DatabasePreparingBase
    {
        [Test]
        public void AllowedItemBelowRoot()
        {
            Definitions.IntegrityStartPage root = CreateRoot();
            ContentItem item = CreateItemBelow(root, typeof(Definitions.IntegrityPage));

            Assert.IsNotNull(item);
        }

        [Test]
        public void AllowedPropagatesToSubclasses()
        {
            ContentItem root = CreateItemBelow(null, typeof(Definitions.IntegrityAlternativeStartPage));
            ContentItem item = CreateItemBelow(root, typeof(Definitions.IntegrityPage));

            Assert.IsNotNull(item);
        }

        [Test]
        public void AllowedItemBelowSubClassOfRoot()
        {
            ContentItem root = CreateItemBelow(null, typeof(Definitions.IntegrityAlternativeStartPage));
            ContentItem item = CreateItemBelow(root, typeof(Definitions.IntegrityAlternativePage));

            Assert.IsNotNull(item);
        }

        [Test]
        [Obsolete]
        public void ValidateRootDefinition()
        {
            ItemDefinition rootDefinition = engine.Definitions.GetDefinition(typeof(Definitions.IntegrityRoot));
            ItemDefinition startPageDefinition = engine.Definitions.GetDefinition(typeof(Definitions.IntegrityStartPage));
            var all = engine.Definitions.GetDefinitions().ToArray();

            Assert.That(rootDefinition.GetAllowedChildren(engine.Definitions, null).Contains(startPageDefinition));
            //EnumerableAssert.Contains(rootDefinition.AllowedChildren, startPageDefinition);
            Assert.IsNull(rootDefinition.AuthorizedRoles);
            Assert.AreEqual(0, rootDefinition.AvailableZones.Count);
            Assert.AreEqual(0, rootDefinition.Containers.Count);
            Assert.IsEmpty(rootDefinition.Description);
            Assert.AreEqual(typeof(Definitions.IntegrityRoot).Name, rootDefinition.Discriminator);
            Assert.That(rootDefinition.Displayables.Count, Is.EqualTo(20));
            Assert.AreEqual(0, rootDefinition.Editables.Count);
            EnumerableAssert.Contains(engine.Definitions.GetAllowedChildren(new Definitions.IntegrityRoot(), null, null), startPageDefinition);
            Assert.AreEqual(0, rootDefinition.Editables.Count);
            Assert.AreEqual(0, rootDefinition.GetModifiers("Title").Count); 
            Assert.AreEqual(0, rootDefinition.EditableModifiers.Count);
            Assert.AreEqual(0, rootDefinition.SortOrder);
            Assert.AreEqual(typeof(Definitions.IntegrityRoot).Name, rootDefinition.Title);
        }

        [Test]
        public void AllowedChildren_Superceedes_NoAllowedParents()
        {
            ContentItem realRoot = CreateItemBelow(null, typeof(Definitions.IntegrityRoot));
            ContentItem item = CreateItemBelow(realRoot, typeof(Definitions.IntegrityStartPage));
            
            Assert.IsNotNull(item);
        }

        private Definitions.IntegrityStartPage CreateRoot()
        {
            return (Definitions.IntegrityStartPage)CreateItemBelow(null, typeof(Definitions.IntegrityStartPage));
        }

        private ContentItem CreateItemBelow(ContentItem parent, Type itemType)
        {
            ContentItem item = engine.Resolve<ContentActivator>().CreateInstance(itemType, parent);
            engine.Persister.Save(item);
            return item;
        }
    }
}
