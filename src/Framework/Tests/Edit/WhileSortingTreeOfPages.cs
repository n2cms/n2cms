using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Edit;
using N2.Persistence;
using N2.Tests.Edit.Items;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using N2.Details;

namespace N2.Tests.Edit
{
	[TestFixture]
	public class WhileSortingTreeOfPages : ItemTestsBase
	{
		TreeSorter sorter;
		NormalPage root, page1, page2, page3;
		ContentPersister persister;
		FakeRepository<ContentItem> repository;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			FakeRepository<ContentDetail> linkRepository;
			persister = TestSupport.SetupFakePersister(out repository, out linkRepository);
			var webContext = new ThreadContext();

			IEditManager editManager = mocks.Stub<IEditManager>();
			Expect.Call(editManager.GetEditorFilter(null))
				.IgnoreArguments()
				.Return(new PageFilter());
			mocks.ReplayAll();

			root = CreateOneItem<NormalPage>(1, "root", null);
			page1 = CreateOneItem<NormalPage>(2, "page1", root);
			page2 = CreateOneItem<NormalPage>(3, "page2", root);
			page2.SortOrder = 1;
			page3 = CreateOneItem<NormalPage>(4, "page3", root);
			page3.SortOrder = 2;
			
			sorter = new TreeSorter(persister, editManager, webContext);
		}

		[Test]
		public void SortUp_FromBottom_IsMovedOneStep()
		{
			sorter.MoveUp(page3);
			Assert.That(root.Children[1], Is.EqualTo(page3));
		}

		[Test]
		public void SortUp_FromMiddle_IsMovedOneStep()
		{
			sorter.MoveUp(page2);
			Assert.That(root.Children[0], Is.EqualTo(page2));
		}

		[Test]
		public void SortUp_FromTop_IsNotMoved()
		{
			sorter.MoveUp(page1);
			Assert.That(root.Children[0], Is.EqualTo(page1));
		}

		[Test]
		public void SortDown_FromBottom_IsNotMoved()
		{
			sorter.MoveDown(page3);
			Assert.That(root.Children[2], Is.EqualTo(page3));
		}

		[Test]
		public void SortDown_FromMiddle_IsMovedOneStep()
		{
			sorter.MoveDown(page2);
			Assert.That(root.Children[2], Is.EqualTo(page2));
		}

		[Test]
		public void SortDown_FromTop_IsMovedOneStep()
		{
			sorter.MoveDown(page1);
			Assert.That(root.Children[1], Is.EqualTo(page1));
		}

		[Test]
		public void SortUp_ChangedItems_GetsTheirSortOrderUpdated()
		{
			sorter.MoveUp(page3);
			Assert.That(page3.SortOrder, Is.GreaterThan(page1.SortOrder));
			Assert.That(page3.SortOrder, Is.LessThan(page2.SortOrder));
		}

		[Test]
		public void SortUp_ChangedItems_AreSaved()
		{
			sorter.MoveUp(page3);

			//Assert.That(savedItems.IndexOf(page2), Is.GreaterThanOrEqualTo(0));
			
		}

		[Test]
		public void SortDown_ChangedItems_GetsTheirSortOrderUpdated()
		{
			sorter.MoveDown(page1);
			Assert.That(page1.SortOrder, Is.GreaterThan(page2.SortOrder));
			Assert.That(page1.SortOrder, Is.LessThan(page3.SortOrder));
		}

		[Test]
		public void SortDown_ChangedItems_AreSaved()
		{
			sorter.MoveDown(page1);

			repository.database.Count.ShouldBe(1);
			Assert.That(page1.SortOrder, Is.GreaterThan(page2.SortOrder));
		}

		[Test]
		public void LastPage_MoveTo_BeforeFirst_IsMovedToFirst()
		{
			sorter.MoveTo(page3, NodePosition.Before, page1);
			Assert.That(root.Children[0], Is.EqualTo(page3));
		}

		[Test]
		public void LastPage_MovedTo_AfterFirst_IsMovedTo_AfterFirst()
		{
			sorter.MoveTo(page3, NodePosition.After, page1);
			Assert.That(root.Children[1], Is.EqualTo(page3));
		}

		[Test]
		public void LastPage_MoveTo_BeforeMiddle_IsMovedTo_BeforeMiddle()
		{
			sorter.MoveTo(page3, NodePosition.Before, page2);
			Assert.That(root.Children[1], Is.EqualTo(page3));
		}

		[Test]
		public void LastPage_MoveTo_AfterMiddle_IsNotMoved()
		{
			sorter.MoveTo(page3, NodePosition.After, page2);
			Assert.That(root.Children[2], Is.EqualTo(page3));
		}

		[Test]
		public void LastPage_MoveTo_BeforeItself_IsNotMoved()
		{
			sorter.MoveTo(page3, NodePosition.Before, page3);
			Assert.That(root.Children[2], Is.EqualTo(page3));
		}

		[Test]
		public void LastPage_MoveTo_AfterItself_IsNotMoved()
		{
			sorter.MoveTo(page3, NodePosition.After, page3);
			Assert.That(root.Children[2], Is.EqualTo(page3));
		}

		[Test]
		public void FirstPage_MovedTo_BeforeItself_IsNotMoved()
		{
			sorter.MoveTo(page1, NodePosition.Before, page1);
			Assert.That(root.Children[0], Is.EqualTo(page1));
		}

		[Test]
		public void FirstPage_MovedTo_AfetrItself_IsNotMoved()
		{
			sorter.MoveTo(page1, NodePosition.After, page1);
			Assert.That(root.Children[0], Is.EqualTo(page1));
		}

		[Test]
		public void FirstPage_MovedTo_BeforeMiddle_IsNotMoved()
		{
			sorter.MoveTo(page1, NodePosition.Before, page2);
			Assert.That(root.Children[0], Is.EqualTo(page1));
		}

		[Test]
		public void FirstPage_MovedTo_AfetrMiddle_IsMovedToAfter()
		{
			sorter.MoveTo(page1, NodePosition.After, page2);
			Assert.That(root.Children[1], Is.EqualTo(page1));
		}

		[Test]
		public void FirstPage_MovedTo_BeforeLast_IsMovedToBeforeLast()
		{
			sorter.MoveTo(page1, NodePosition.Before, page3);
			Assert.That(root.Children[1], Is.EqualTo(page1));
		}

		[Test]
		public void FirstPage_MovedTo_AfterLast_IsMovedToLast()
		{
			sorter.MoveTo(page1, NodePosition.After, page3);
			Assert.That(root.Children[2], Is.EqualTo(page1));
		}

        [Test]
        public void CanMove_NewItem_WithSameParent_ToBefore_FirstItem()
        {
            var page0 = CreateOneItem<NormalPage>(0, "page0", null);
            page0.Parent = root;
            sorter.MoveTo(page0, NodePosition.Before, page1);
            Assert.That(root.Children[0], Is.EqualTo(page0));
            Assert.That(root.Children[1], Is.EqualTo(page1));
        }

        [Test]
        public void CanMove_NewItem_WithSameParent_ToAfter_LastItem()
        {
            var page4 = CreateOneItem<NormalPage>(0, "page0", null);
            page4.Parent = root;
            sorter.MoveTo(page4, NodePosition.After, page3);
            Assert.That(root.Children[2], Is.EqualTo(page3));
            Assert.That(root.Children[3], Is.EqualTo(page4));
        }

		[Test]
		public void PuttingItemFirst_DoesntSave_SubsequentPages()
		{
			var page0 = CreateOneItem<NormalPage>(0, "page0", null);
			page0.Parent = root;
			page0.SortOrder = 1000;
			sorter.MoveTo(page0, NodePosition.Before, page1);
			repository.database.Count.ShouldBe(1);
			repository.database.ContainsValue(page0).ShouldBe(true);
		}
	}
}
