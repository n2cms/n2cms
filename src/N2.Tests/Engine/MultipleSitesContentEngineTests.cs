using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Engine;
using N2.Web;
using System.Configuration;
using NUnit.Framework.SyntaxHelpers;
using System.IO;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class MultipleSitesContentEngineTests
	{
		ContentEngine engine;
        IHost host;

		[SetUp]
		public void SetUp()
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Engine\\MultipleSites.exe");
			var cfg = ConfigurationManager.OpenExeConfiguration(path);
			engine = new ContentEngine(cfg);
            host = engine.Resolve<IHost>();
		}

		[Test]
		public void MultipleSitesParser_IsConfigurable()
		{
			var parser = engine.Resolve<IUrlParser>();
			Assert.That(parser, Is.TypeOf(typeof(MultipleSitesParser)));
        }

        [Test]
        public void MultipleSitesParser_LoadsConfiguredSites()
        {
            var parser = (MultipleSitesParser)engine.Resolve<IUrlParser>();
            Assert.That(host.Sites.Count, Is.EqualTo(4));
        }

        [Test]
        public void MultipleSitesParser_HonorsWildcardsMapping()
        {
            var parser = (MultipleSitesParser)engine.Resolve<IUrlParser>();
            Assert.That(host.Sites[0].Is("alpha.localhost.com"), Is.True);
            Assert.That(host.Sites[0].Is("www.alpha.localhost.com"), Is.False);
            Assert.That(host.Sites[3].Is("mysite.com"), Is.True);
            Assert.That(host.Sites[3].Is("www.mysite.com"), Is.True);
        }

        [Test]
        public void MultipleSitesParser_CanConfigureWildcardmapping_PerSite()
        {
            var parser = (MultipleSitesParser)engine.Resolve<IUrlParser>();
            Assert.That(host.Sites[0].Wildcards, Is.False);
            Assert.That(host.Sites[1].Wildcards, Is.False);
            Assert.That(host.Sites[2].Wildcards, Is.False);
            Assert.That(host.Sites[3].Wildcards, Is.True);
        }

        [Test]
        public void MultipleSitesParser_Loads_SiteSettings()
        {
            var parser = (MultipleSitesParser)engine.Resolve<IUrlParser>();
            Assert.That(host.Sites[0].Settings["nextSite"], Is.EqualTo("http://beta.localhost.com"));
        }
	}
}
