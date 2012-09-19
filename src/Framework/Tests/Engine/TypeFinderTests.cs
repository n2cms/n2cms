using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using NUnit.Framework;
using Shouldly;
using N2.Web;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class TypeFinderTests
	{
		private TypeCache assemblyCache;
		private WebAppTypeFinder typeFinder;

		[SetUp]
		public void SetUp()
		{
			assemblyCache = new TypeCache(new N2.Persistence.BasicTemporaryFileHelper(new ThreadContext()));
			var config = new N2.Configuration.EngineSection();
			config.Assemblies.SkipLoadingPattern = "nothing";
			config.Assemblies.Remove(new N2.Configuration.AssemblyElement("N2.Management"));
			config.Assemblies.EnableTypeCache = false;
			typeFinder = new WebAppTypeFinder(assemblyCache, config);
		}

		[Test]
		public void Finds_SingleType()
		{
			typeFinder.Find(typeof(TypeFinderTests)).Single().ShouldBe(typeof(TypeFinderTests));
		}

		[Test]
		public void Finds_AttributedType()
		{
			var at = typeFinder.Find<TestFixtureAttribute>(typeof(TypeFinderTests)).Single();
			at.Attribute.ShouldNotBe(null);
			at.Type.ShouldBe(typeof(TypeFinderTests));
		}


		[Test]
		public void Finds_SingleType_FromCache()
		{
			typeFinder.Find(typeof(ItemTestsBase)); // buildup cache
			typeFinder.Find(typeof(ItemTestsBase)).Count().ShouldBeGreaterThan(120);
		}

		[Test]
		public void Finds_AttributedType_FromCache()
		{
			typeFinder.Find<TestFixtureAttribute>(typeof(ItemTestsBase)); // buildup cache
			var ats = typeFinder.Find<TestFixtureAttribute>(typeof(ItemTestsBase));
			ats.Count().ShouldBeGreaterThan(100);
		}
	}
}
