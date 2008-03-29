using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using N2.Tests.Collections;

namespace N2.Tests.Persistence
{
	[TestFixture]
	public class FindTests : ItemTestsBase
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			BuildHierarchy();
		}

		ContentItem a, a_a, a_b, a_a_a, a_a_b, a_b_a, a_b_b;
		private void BuildHierarchy()
		{
			int i = 0;
			a = CreateOneItem<FirstItem>(++i, "a", null);
			a_a = CreateOneItem<FirstItem>(++i, "a_a", a);
			a_b = CreateOneItem<FirstItem>(++i, "a_b", a);

			a_a_a = CreateOneItem<FirstItem>(++i, "a_a_a", a_a);
			a_a_b = CreateOneItem<FirstItem>(++i, "a_a_b", a_a);

			a_b_a = CreateOneItem<FirstItem>(++i, "a_b_a", a_b);
			a_b_b = CreateOneItem<FirstItem>(++i, "a_b_b", a_b);
		}

		[Test]
		public void EnumerateParents_FindDoesNotPassLastItem()
		{
			EnumerableAssert.Count(1, Find.EnumerateParents(a_a_a, a_a));
		}

		[Test]
		public void EnumerateParents_CanAcceptNullAsLastItem()
		{
			EnumerableAssert.Count(2, Find.EnumerateParents(a_a_a, null));
		}

		[Test]
		public void EnumerateParents_FindsParentsOnTwoLevels()
		{
			EnumerableAssert.Count(2, Find.EnumerateParents(a_a_a, null));
		}

		[Test]
		public void EnumerateParents_FindDoesNotPassLastItem_WhenInitialItemIsLast()
		{
			EnumerableAssert.Count(0, Find.EnumerateParents(a_a, a_a));
		}

		[Test]
		public void EnumerateParents_CanIncludeSelf_Same_InitialAndLast()
		{
			EnumerableAssert.Count(1, Find.EnumerateParents(a_a, a_a, true));
		}

		[Test]
		public void EnumerateParents_CanIncludeSelf_Different_InitialAndLast()
		{
			EnumerableAssert.Count(2, Find.EnumerateParents(a_a, a, true));
		}

		[Test, ExpectedArgumentNullException]
		public void EnumerateParents_BoltsOnNullInitialItem()
		{
			EnumerableAssert.Count(0, Find.EnumerateParents(null, a_a));
		}

		[Test]
		public void IsDescendant_ReturnsTrue_WhenGivenGrandParent()
		{
			bool isDescendant = Find.IsDescendant(a_a_a, a);
			Assert.IsTrue(isDescendant);
		}

		[Test]
		public void IsDescendant_ReturnsTrue_WhenGivenParent()
		{
			bool isDescendant = Find.IsDescendant(a_a_a, a_a);
			Assert.IsTrue(isDescendant);
		}

		[Test]
		public void IsDescendant_ReturnsFalse_WhenGivenItself()
		{
			bool isDescendant = Find.IsDescendant(a_a_a, a_a_a);
			Assert.IsFalse(isDescendant);
		}

		[Test]
		public void IsDescendant_ReturnsFalse_WhenGivenItemInOtherBranch()
		{
			bool isDescendant = Find.IsDescendant(a_a_a, a_b_a);
			Assert.IsFalse(isDescendant);
		}

		[Test]
		public void IsDescendant_ReturnsFalse_WhenGivenItemBelowItself()
		{
			bool isDescendant = Find.IsDescendant(a_a, a_a_a);
			Assert.IsFalse(isDescendant);
		}

		[Test]
		public void IsDescendantOrSelf_ReturnsTrue_WhenGivenItself()
		{
			bool isDescendant = Find.IsDescendantOrSelf(a_a_a, a_a_a);
			Assert.IsTrue(isDescendant);
		}

		[Test]
		public void IsDescendantOrSelf_ReturnsTrue_WhenGivenParent()
		{
			bool isDescendant = Find.IsDescendantOrSelf(a_a_a, a_a);
			Assert.IsTrue(isDescendant);
		}

		[Test]
		public void In_ReturnsTrue_WhenItemIsInEnumeration()
		{
			bool isIn = Find.In(a, new ContentItem[] {a_a_a, a_a, a, a_b, a_b_b});
			Assert.IsTrue(isIn);
		}

		[Test]
		public void In_ReturnsFalse_WhenItemIsNotInEnumeration()
		{
			bool isIn = Find.In(a_a_b, new ContentItem[] { a_a_a, a_a, a, a_b, a_b_b });
			Assert.IsFalse(isIn);
		}
	}
}
