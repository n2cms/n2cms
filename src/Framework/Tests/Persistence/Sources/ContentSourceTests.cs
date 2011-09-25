using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence.Sources;

namespace N2.Tests.Persistence.Sources
{
	[TestFixture]
	public class ContentSourceTests
	{
		[Test]
		public void DatabaseSource_IsOrderedLast()
		{
			ContentSource cs = new ContentSource(new Fakes.FakeSecurityManager(), new DatabaseSource(null, null), new ActiveContentSource());

			Assert.That(cs.Sources.Last(), Is.TypeOf<DatabaseSource>());
		}
	}
}
