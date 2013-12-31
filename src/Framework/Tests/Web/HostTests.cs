using N2.Configuration;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class HostTests : ItemTestsBase
    {
        HostSection config;
        ThreadContext ctx;
        Host host;
        ContentItem root, start;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            config = new HostSection();
            config.RootID = 123;
            config.StartPageID = 234;
            ctx = new ThreadContext();
            host = new Host(ctx, config);

            root = CreateOneItem<Items.PageItem>(123, "start", null);
            start = CreateOneItem<Items.PageItem>(234, "start", null);
        }



        [Test]
        public void DefaultSite_DerivesFromConfig()
        {
            var site = host.DefaultSite;
            Assert.That(site.RootItemID, Is.EqualTo(123));
            Assert.That(site.StartPageID, Is.EqualTo(234));
        }

        [Test]
        public void Sites_AreRead_FromConfig()
        {
            config.Sites.Add(new SiteElement { ID = 444, Name = "n2cms.com" });
            host = new Host(ctx, config);

            var site = host.GetSite(444);

            Assert.That(site.StartPageID, Is.EqualTo(444));
            Assert.That(site.Authority, Is.EqualTo("n2cms.com"));
        }

        [Test]
        public void GetSite_WithKnownSite_GivesSite()
        {
            host.AddSites(new[] { new Site(123, 235, "n2cms.com") });

            var site = host.GetSite("http://n2cms.com");

            Assert.That(site, Is.Not.Null);
            Assert.That(site.RootItemID, Is.EqualTo(123));
            Assert.That(site.StartPageID, Is.EqualTo(235));
            Assert.That(site.Authority, Is.EqualTo("n2cms.com"));
        }

        [Test]
        public void GetSite_WithKnownSiteSubdomain_GivesSite_WithWildcards()
        {
            host.AddSites(new[] { new Site(123, 235, "n2cms.com") { Wildcards = true } });

            var site = host.GetSite("http://www.n2cms.com");

            Assert.That(site, Is.Not.Null);
            Assert.That(site.RootItemID, Is.EqualTo(123));
            Assert.That(site.StartPageID, Is.EqualTo(235));
            Assert.That(site.Authority, Is.EqualTo("n2cms.com"));
        }

        [Test]
        public void GetSite_WithKnownSiteSubdomain_DoesntGiveSite_WithNoWildcards()
        {
            host.AddSites(new[] { new Site(123, 235, "n2cms.com") { Wildcards = false } });

            var site = host.GetSite("http://www.n2cms.com");

            Assert.That(site, Is.Null);
        }

        [Test]
        public void GetSite_WithUnknownSite_GivesNull()
        {
            var site = host.GetSite("http://www.n2cms.com");

            Assert.That(site, Is.Null);
        }

        [Test]
        public void CurrentSite_FallbacksTo_DefaultSite()
        {
            var site = host.CurrentSite;

            Assert.That(site.RootItemID, Is.EqualTo(123));
            Assert.That(site.StartPageID, Is.EqualTo(234));
        }

        [Test]
        public void GetSite_OfAnItem_GetsTheItems_Site()
        {
            var item = CreateOneItem<Items.PageItem>(444, "item", start);

            var site = host.GetSite(item);

            Assert.That(site.RootItemID, Is.EqualTo(123));
            Assert.That(site.StartPageID, Is.EqualTo(234));
        }

        [Test]
        public void GetSite_OfAnId_ThatIsASite_GivesTheSite()
        {
            var site = host.GetSite(234);

            Assert.That(site.RootItemID, Is.EqualTo(123));
            Assert.That(site.StartPageID, Is.EqualTo(234));
        }

        [Test]
        public void GetSite_OfAnId_ThatIsNotASite_GivesNull()
        {
            var site = host.GetSite(444);

            Assert.That(site, Is.Null);
        }

        [Test]
        public void AddSites_AddsTheSites()
        {
            host.AddSites(new[] { new Site(555) });

            var site = host.GetSite(555);

            Assert.That(site, Is.Not.Null);
        }

        [Test]
        public void AddSites_AddsTheSites_ToExistingSites()
        {
            host.AddSites(new[] { new Site(555) });
            host.AddSites(new[] { new Site(666) });

            var site = host.GetSite(555);
            var site2 = host.GetSite(666);

            Assert.That(site, Is.Not.Null);
            Assert.That(site2, Is.Not.Null);
        }

        [Test]
        public void ReplaceSites_RemovesPrevious_DefaultSite()
        {
            host.ReplaceSites(new Site(666), new[] { new Site(777) });

            var site = host.DefaultSite;
            var siteOld = host.GetSite(234);
            var siteNew = host.GetSite(666);
            
            Assert.That(siteOld, Is.Null);
            Assert.That(siteNew, Is.Not.Null);
            Assert.That(site.StartPageID, Is.EqualTo(666));
        }

        [Test]
        public void ReplaceSites_RemovesPrevious_Sites()
        {
            host.AddSites(new[] { new Site(345) });
            host.ReplaceSites(new Site(666), new[] { new Site(777) });

            var siteOld = host.GetSite(345);
            var siteNew = host.GetSite(777);

            Assert.That(siteOld, Is.Null);
            Assert.That(siteNew, Is.Not.Null);
        }
    }
}
