using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Tests.Edit.Items;
using N2.Persistence;
using N2.Edit;
using N2.Definitions;
using N2.Web.UI;
using N2.Details;
using NUnit.Framework.SyntaxHelpers;
using System.Web;

namespace N2.Tests.Edit
{
    [TestFixture]
    public class WhileNavigatingToTheEditPage : EditManagerTests
    {
        [Test]
        public void GetEditNewPageUrl_After()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);

            string editUrl = this.editManager.GetEditNewPageUrl(item, new ItemDefinition(typeof(ComplexContainersItem)), null, CreationPosition.After);

            Assert.That(editUrl, Is.EqualTo("/edit/edit.aspx?selected=" + HttpUtility.UrlEncode(root.Path) + "&discriminator=ComplexContainersItem&zoneName=&after=" + HttpUtility.UrlEncode(item.Path)));
        }

        [Test]
        public void GetEditNewPageUrl_Before()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);

            string editUrl = this.editManager.GetEditNewPageUrl(item, new ItemDefinition(typeof(ComplexContainersItem)), null, CreationPosition.Before);

            Assert.That(editUrl, Is.EqualTo("/edit/edit.aspx?selected=" + HttpUtility.UrlEncode(root.Path) + "&discriminator=ComplexContainersItem&zoneName=&before=" + HttpUtility.UrlEncode(item.Path)));
        }

        [Test]
        public void GetEditNewPageUrl_Below()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);

            string editUrl = this.editManager.GetEditNewPageUrl(item, new ItemDefinition(typeof(ComplexContainersItem)), null, CreationPosition.Below);

            Assert.That(editUrl, Is.EqualTo("/edit/edit.aspx?selected=" + HttpUtility.UrlEncode("/child/") + "&discriminator=ComplexContainersItem&zoneName="));
        }

        [Test]
        public void GetEditUrl_OfPublishedRoot_UsesPath()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            string editUrl = this.editManager.GetEditExistingItemUrl(root);

            Assert.AreEqual("~/edit/edit.aspx?selected=/", editUrl);
        }

        [Test]
        public void GetEditUrl_OfPublishedSubPage_UsesPath()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);
            string editUrl = this.editManager.GetEditExistingItemUrl(item);

            Assert.AreEqual("~/edit/edit.aspx?selected=/child/", editUrl);
        }

        [Test]
        public void GetEditUrl_OfUnpublishedVersion_RevertsToIdentity()
        {
            ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
            ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);
            ContentItem versionOfItem = CreateOneItem<ComplexContainersItem>(3, "child", null);
            versionOfItem.VersionOf = item;

            string editUrl = this.editManager.GetEditExistingItemUrl(versionOfItem);

            Assert.That(editUrl, Is.EqualTo("~/edit/edit.aspx?selectedUrl=" + HttpUtility.UrlEncode("/default.aspx?page=3")));
        }
    }
}
