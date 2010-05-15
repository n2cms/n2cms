using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Configuration;

namespace N2.Tests.Configuration
{
	[TestFixture]
	public class LazyRemovableCollectionTests
	{
		[Test]
		public void CanAddElement()
		{
			AssemblyCollection assemblies = new AssemblyCollection();
			assemblies.Add(new AssemblyElement { Assembly = "N2" });

			Assert.That(assemblies.AddedElements.Count(), Is.EqualTo(1));
			Assert.That(assemblies.AddedElements.First().Assembly, Is.EqualTo("N2"));
		}

		[Test]
		public void CanAddDefaultElement()
		{
			AssemblyCollection assemblies = new AssemblyCollection();
			assemblies.AddDefault(new AssemblyElement { Assembly = "N2" });

			Assert.That(assemblies.AddedElements.Count(), Is.EqualTo(1));
			Assert.That(assemblies.AddedElements.First().Assembly, Is.EqualTo("N2"));
		}

		[Test]
		public void CanRemoveElement()
		{
			AssemblyCollection assemblies = new AssemblyCollection();
			assemblies.Add(new AssemblyElement { Assembly = "N2" });
			assemblies.Remove(new AssemblyElement { Assembly = "N2" });

			Assert.That(assemblies.AddedElements.Count(), Is.EqualTo(0));
		}

		[Test]
		public void CanRemoveDefaultElement()
		{
			AssemblyCollection assemblies = new AssemblyCollection();
			assemblies.AddDefault(new AssemblyElement { Assembly = "N2" });
			assemblies.Remove(new AssemblyElement { Assembly = "N2" });

			Assert.That(assemblies.AddedElements.Count(), Is.EqualTo(0));
		}

		[Test]
		public void CanClearElement()
		{
			AssemblyCollection assemblies = new AssemblyCollection();
			assemblies.Add(new AssemblyElement { Assembly = "N2" });
			assemblies.Clear();

			Assert.That(assemblies.AddedElements.Count(), Is.EqualTo(0));
			Assert.That(assemblies.IsCleared);
		}

		[Test]
		public void CanClearDefaultElement()
		{
			AssemblyCollection assemblies = new AssemblyCollection();
			assemblies.AddDefault(new AssemblyElement { Assembly = "N2" });
			assemblies.Clear();

			Assert.That(assemblies.AddedElements.Count(), Is.EqualTo(0));
			Assert.That(assemblies.IsCleared);
		}
	}
}
