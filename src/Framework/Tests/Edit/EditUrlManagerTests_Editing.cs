using System.Web;
using N2.Definitions;
using N2.Edit;
using N2.Tests.Edit.Items;
using NUnit.Framework;

namespace N2.Tests.Edit
{
    [TestFixture]
    public class EditUrlManagerTests_Editing : EditUrlManagerTests
    {
        [Test]
        public void GetEditNewPageUrl_After()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);

            string editUrl = editUrlManager.GetEditNewPageUrl(item, new ItemDefinition(typeof (ComplexContainersItem)), null,
                                                              CreationPosition.After);

            Assert.That(editUrl,
                        Is.EqualTo("/N2/Content/Edit.aspx?selected=" + HttpUtility.UrlEncode(root.Path) +
                                   "&discriminator=ComplexContainersItem&after=" + HttpUtility.UrlEncode(item.Path)));
        }

        [Test]
        public void GetEditNewPageUrl_Before()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);

            string editUrl = editUrlManager.GetEditNewPageUrl(item, new ItemDefinition(typeof (ComplexContainersItem)), null,
                                                              CreationPosition.Before);

            Assert.That(editUrl,
                        Is.EqualTo("/N2/Content/Edit.aspx?selected=" + HttpUtility.UrlEncode(root.Path) +
                                   "&discriminator=ComplexContainersItem&before=" + HttpUtility.UrlEncode(item.Path)));
        }

        [Test]
        public void GetEditNewPageUrl_Below()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);

            string editUrl = editUrlManager.GetEditNewPageUrl(item, new ItemDefinition(typeof (ComplexContainersItem)), null,
                                                              CreationPosition.Below);

            Assert.That(editUrl,
                        Is.EqualTo("/N2/Content/Edit.aspx?selected=" + HttpUtility.UrlEncode("/child/") +
                                   "&discriminator=ComplexContainersItem"));
        }

        [Test]
        public void GetEditUrl_OfPublishedRoot_UsesPath()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            string editUrl = editUrlManager.GetEditExistingItemUrl(root);

            Assert.AreEqual("/N2/Content/Edit.aspx?selected=%2f", editUrl);
        }

        [Test]
        public void GetEditUrl_OfPublishedSubPage_UsesPath()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);
            string editUrl = editUrlManager.GetEditExistingItemUrl(item);

            Assert.AreEqual("/N2/Content/Edit.aspx?selected=%2fchild%2f", editUrl);
        }

        [Test]
        public void GetEditUrl_OfUnpublishedVersion_AppendsVersionIndex_ToMasterVersionPath()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);
            ContentItem versionOfItem = CreateOneItem<ComplexContainersItem>(3, "child", null);
            versionOfItem.VersionOf = item;
            versionOfItem.VersionIndex = 33;

            string editUrl = editUrlManager.GetEditExistingItemUrl(versionOfItem);

            Assert.That(editUrl, Is.EqualTo("/N2/Content/Edit.aspx?selected=" + HttpUtility.UrlEncode(item.Path) + "&n2versionIndex=33"));
        }
    }
}
