using NUnit.Framework;
using N2.Edit.Trash;
using N2.Tests;

namespace N2.Extensions.Tests.Extensions
{
	[TestFixture]
	public class ExtensionTests : ItemTestsBase
	{
		[Test]
		public void CanDetect_ItemInTrashcan()
		{
			var root = CreateOneItem<MyItem>(1, "root", null);
			var trash = CreateOneItem<MyTrash>(2, "trash", root);
			var item = CreateOneItem<MyTrash>(3, "item", trash);

			bool isRecycled = item.IsRecycled();
			
			Assert.That(isRecycled, Is.True);
		}

		[Test]
		public void CanDetect_ItemFurtherDown_InTrashcan()
		{
			var root = CreateOneItem<MyItem>(1, "root", null);
			var trash = CreateOneItem<MyTrash>(2, "trash", root);
			var item = CreateOneItem<MyTrash>(3, "item", trash);
			var deepItem = CreateOneItem<MyTrash>(3, "deep item", item);

			bool isRecycled = deepItem.IsRecycled();

			Assert.That(isRecycled, Is.True);
		}

		[Test]
		public void CanDetect_ItemNotInTrashcan()
		{
			var root = CreateOneItem<MyItem>(1, "root", null);
			var item = CreateOneItem<MyTrash>(3, "item", root);

			bool isRecycled = item.IsRecycled();

			Assert.That(isRecycled, Is.False);
		}



		class MyItem : ContentItem
		{
		}
		class MyTrash : ContentItem, ITrashCan
		{
			#region ITrashCan Members

			public bool Enabled
			{
				get { return true; }
			}

			#endregion
		}
	}
}
