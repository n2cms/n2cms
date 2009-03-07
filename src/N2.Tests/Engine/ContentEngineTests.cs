using NUnit.Framework;
using N2.Engine;
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
			engine = new ContentEngine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None), "n2", EventBroker.Instance);
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
			Assert.That(host.Sites[0].Authority, Is.EqualTo("alpha.localhost.com"));
			Assert.That(host.Sites[0].StartPageID, Is.EqualTo(4));
		}
	}
}
