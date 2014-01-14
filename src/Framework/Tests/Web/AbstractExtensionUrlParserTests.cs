using N2.Tests.Web.Items;
using NUnit.Framework;

namespace N2.Tests.Web
{
    public abstract class AbstractExtensionUrlParserTests : ParserTestsBase
    {
        ContentItem ext3;
        ContentItem ext3_1;
        ContentItem item3_2;
        ContentItem ext2_2;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CreateDefaultStructure();

            ext2_2 = CreateOneItem<CustomExtensionPage>(9, "ext2_2", page2);
            ext3 = CreateOneItem<CustomExtensionPage>(10, "ext3", startItem);
            ext3_1 = CreateOneItem<CustomExtensionPage>(11, "ext3_1", ext3);
            item3_2 = CreateOneItem<PageItem>(12, "item3_2", ext3);
        }

        [Test]
        public void CanParse_ExtendedItem()
        {
            ContentItem found = parser.Parse("/ext3" + Items.CustomExtensionPage.extension);
            Assert.That(found, Is.EqualTo(ext3));
        }

        [Test]
        public void CanParse_ExtendedItem_BelowExtendedItem()
        {
            ContentItem found = parser.Parse("/ext3/ext3_1" + Items.CustomExtensionPage.extension);
            Assert.That(found, Is.EqualTo(ext3_1));
        }

        [Test]
        public void CanParse_NotExtendedItem_BelowExtendedItem()
        {
            ContentItem found = parser.Parse("/ext3/item3_2.aspx");
            Assert.That(found, Is.EqualTo(item3_2));
        }

        [Test]
        public void CanParse_ExtendedItem_BelowNotExtendedItem()
        {
            ContentItem found = parser.Parse("/item2/ext2_2" + Items.CustomExtensionPage.extension);
            Assert.That(found, Is.EqualTo(ext2_2));
        }
    }
}
