using System.Linq;
using N2.Configuration;
using NUnit.Framework;

namespace N2.Tests.Configuration
{
    public class NamedCollectionTester : LazyRemovableCollection<NamedElement>
    {
    }

    [TestFixture]
    public class LazyRemovableCollectionTests
    {
        [Test]
        public void CanAddElement()
        {
            NamedCollectionTester assemblies = new NamedCollectionTester();
            assemblies.Add(new NamedElement { Name = "N2" });

            Assert.That(assemblies.AllElements.Count(), Is.EqualTo(1));
            Assert.That(assemblies.Count, Is.EqualTo(1));
            Assert.That(assemblies.AllElements.First().Name, Is.EqualTo("N2"));
        }

        [Test]
        public void CanAddDefaultElement()
        {
            NamedCollectionTester assemblies = new NamedCollectionTester();
            assemblies.AddDefault(new NamedElement { Name = "N2" });

            Assert.That(assemblies.AllElements.Count(), Is.EqualTo(1));
            Assert.That(assemblies.Count, Is.EqualTo(1));
            Assert.That(assemblies.AllElements.First().Name, Is.EqualTo("N2"));
        }

        [Test]
        public void CanRemoveElement()
        {
            NamedCollectionTester assemblies = new NamedCollectionTester();
            assemblies.Add(new NamedElement { Name = "N2" });
            assemblies.Remove(new NamedElement { Name = "N2" });

            Assert.That(assemblies.AllElements.Count(), Is.EqualTo(0));
            Assert.That(assemblies.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanRemoveDefaultElement()
        {
            NamedCollectionTester assemblies = new NamedCollectionTester();
            assemblies.AddDefault(new NamedElement { Name = "N2" });
            assemblies.Remove(new NamedElement { Name = "N2" });

            Assert.That(assemblies.AllElements.Count(), Is.EqualTo(0));
            Assert.That(assemblies.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanClearElement()
        {
            NamedCollectionTester assemblies = new NamedCollectionTester();
            assemblies.Add(new NamedElement { Name = "N2" });
            assemblies.Clear();

            Assert.That(assemblies.AllElements.Count(), Is.EqualTo(0));
            Assert.That(assemblies.IsCleared);
        }

        [Test]
        public void CanClearDefaultElement()
        {
            NamedCollectionTester assemblies = new NamedCollectionTester();
            assemblies.AddDefault(new NamedElement { Name = "N2" });
            assemblies.Clear();

            Assert.That(assemblies.AllElements.Count(), Is.EqualTo(0));
            Assert.That(assemblies.Count, Is.EqualTo(0));
            Assert.That(assemblies.IsCleared);
        }

        [Test, Ignore("Hmm, 2010?")]
        public void CanEnumerateViewCollection()
        {
            NamedCollectionTester assemblies = new NamedCollectionTester();
            assemblies.Add(new NamedElement { Name = "N2" });
            assemblies.AddDefault(new NamedElement { Name = "N3" });

            Assert.That(assemblies.Count(), Is.EqualTo(2));
            Assert.That(assemblies.Any<NamedElement>(a => a.Name == "N2"));
            Assert.That(assemblies.Any<NamedElement>(a => a.Name == "N3"));
        }
    }
}
