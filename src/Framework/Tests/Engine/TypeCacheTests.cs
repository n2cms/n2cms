using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Engine;
using N2.Web;
using Shouldly;
using System.Reflection;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class TypeCacheTests
	{
		private TypeCache typeCache;

		[SetUp]
		public void SetUp()
		{
			typeCache = CreateTypeCache();
			typeCache.Clear();
		}

		private static TypeCache CreateTypeCache()
		{
			var tc = new ThreadContext();
			return new TypeCache(new N2.Persistence.BasicTemporaryFileHelper(tc));
		}

		[Test]
		public void GetAssembliesFromCached()
		{
			var nonCached = typeCache.GetAssemblies(() => new[] { GetType().Assembly });
			var cached = typeCache.GetAssemblies(() => null);

			cached.ShouldBe(GetTestAssembly());
			cached.Single().ShouldBe(GetType().Assembly);
		}

		[Test]
		public void GetTypesFromCached()
		{
			var nonCached = typeCache.GetTypes("hello", GetTestAssembly, (a) => new[] { typeof(TypeCacheTests) }).ToArray();
			typeCache = CreateTypeCache();
			var cached = typeCache.GetTypes("hello", GetTestAssembly, (a) => null).ToArray();

			cached.Single().ShouldBe(typeof(TypeCacheTests));
		}

		public IEnumerable<Assembly> GetTestAssembly()
		{
			yield return GetType().Assembly;
		}
	}
}
