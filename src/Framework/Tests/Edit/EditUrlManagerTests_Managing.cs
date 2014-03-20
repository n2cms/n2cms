using NUnit.Framework;
using Shouldly;
using N2.Web;

namespace N2.Tests.Edit
{
    [TestFixture]
    public class EditUrlManagerTests_Managing : EditUrlManagerTests
    {
        [Test]
        public void CanGetManagementInterfaceUrl()
        {
            var url = editUrlManager.GetManagementInterfaceUrl();

            Assert.That(url, Is.EqualTo("/N2/"));
        }

        [Test]
        public void CanResolveManagementUrl_WithNullArgument()
        {
            var url = editUrlManager.ResolveResourceUrl(null);

            Assert.That(url, Is.EqualTo("/N2/"));
        }

        [Test]
        public void CanResolveResourceUnderManagementUrl()
        {
            var url = editUrlManager.ResolveResourceUrl("Resources/aresource.css");

            Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
        }

        [Test]
        public void CanResolveResourceUnderManagementUrl_UsingManagementTag()
        {
            var url = editUrlManager.ResolveResourceUrl("{ManagementUrl}/Resources/aresource.css");

            Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
        }

        [Test]
        public void DoesNotDisruptVirtualPaths()
        {
            var url = editUrlManager.ResolveResourceUrl("~/N2/Resources/aresource.css");

            Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
        }

        [Test]
        public void DoesNotDisruptAbsolutePaths()
        {
            var url = editUrlManager.ResolveResourceUrl("/N2/Resources/aresource.css");

            Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
        }

        [TestCase("http://n2cms.com", "http")]
        [TestCase("https://n2cms.com", "https")]
        [TestCase("javascript:alert('hello');", "javascript")]
        [TestCase("file:c:\\autoexec.bat", "file uri")]
        [TestCase("mailto:noone@nowhere.com", "mail")]
        [TestCase("ftp://ftp.sunet.se:21/", "ftp")]
        public void DoesNotDisrupt(string input, string type)
        {
            var result = editUrlManager.ResolveResourceUrl(input);

            Assert.That(result, Is.EqualTo(input), type + " failed");
        }

        [Test]
        public void GetDeleteUrl_SelectsItem()
        {
            var url = editUrlManager.GetDeleteUrl(item);
            url.ShouldBe("/N2/Content/delete.aspx?selected=/item/");
        }

        [Test]
        public void GetDeleteUrl_OfVersion_SelectsMasterItem_AndAppends_VersionIndex()
        {
            var url = editUrlManager.GetDeleteUrl(version);
            url.ShouldBe("/N2/Content/delete.aspx?selected=/item/&n2versionIndex=2&n2versionKey=VERSKEY");
        }
    }
}
