using N2.Tests;
using NUnit.Framework;

namespace N2.Extensions.Tests.Extensions
{
    public abstract class ExtensionTests : ItemTestsBase
    {
        protected MyItem root;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            root = CreateOneItem<MyItem>(1, "root", null);
        }


        protected class MyItem : ContentItem
        {
        }
    }
}
