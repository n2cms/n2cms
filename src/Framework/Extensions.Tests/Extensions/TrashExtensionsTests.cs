using System;
using N2.Edit.Trash;
using NUnit.Framework;

namespace N2.Extensions.Tests.Extensions
{
    [TestFixture]
    public class TrashExtensionsTests : ExtensionTests
    {
        [Test]
        public void CanDetect_ItemInTrashcan()
        {
            var trash = CreateOneItem<MyTrash>(2, "trash", root);
            var item = CreateOneItem<MyTrash>(3, "item", trash);

            bool isRecycled = item.IsRecycled();

            Assert.That(isRecycled, Is.True);
        }

        [Test]
        public void CanDetect_ItemFurtherDown_InTrashcan()
        {
            var trash = CreateOneItem<MyTrash>(2, "trash", root);
            var item = CreateOneItem<MyTrash>(3, "item", trash);
            var deepItem = CreateOneItem<MyTrash>(3, "deep item", item);

            bool isRecycled = deepItem.IsRecycled();

            Assert.That(isRecycled, Is.True);
        }

        [Test]
        public void CanDetect_ItemNotInTrashcan()
        {
            var item = CreateOneItem<MyTrash>(3, "item", root);

            bool isRecycled = item.IsRecycled();

            Assert.That(isRecycled, Is.False);
        }


        protected class MyTrash : ContentItem, ITrashCan
        {
            #region ITrashCan Members

            public bool Enabled
            {
                get { return true; }
            }

            public TrashPurgeInterval PurgeInterval
            {
                get { throw new NotImplementedException(); }
            }

            public bool AsyncTrashPurging
            {
                get { throw new NotImplementedException(); }
            }
            #endregion
        }
    }
}
