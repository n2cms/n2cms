using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Engine;
using N2.Persistence;
using NUnit.Framework.SyntaxHelpers;
using N2.Web;
using N2.Persistence.NH;
using System.Configuration;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class ContentEngineTests
	{
		ContentEngine engine;

		[SetUp]
		public void SetUp()
		{
			engine = new ContentEngine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));
		}

		[Test]
		public void CanAssignRootAndStartPageID()
		{
			var host = engine.Resolve<IHost>();
			Assert.That(host.DefaultSite.RootItemID, Is.EqualTo(2));
			Assert.That(host.DefaultSite.StartPageID, Is.EqualTo(3));
		}

		[Test]
		public void CanAssignDefaultNHibernateProperties()
		{
			var cb = (ConfigurationBuilder)engine.Resolve<IConfigurationBuilder>();
			Assert.That(cb.Properties.Count, Is.GreaterThan(0));
		}

		[Test]
		public void SitesAreSetup()
		{
			var host = engine.Resolve<IHost>();
			Assert.That(host.Sites.Count, Is.EqualTo(3));
			Assert.That(host.Sites[0].Host, Is.EqualTo("alpha.localhost.com"));
			Assert.That(host.Sites[0].StartPageID, Is.EqualTo(4));
		}
	}
}
