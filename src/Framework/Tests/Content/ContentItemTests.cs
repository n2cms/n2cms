using System;
using N2.Details;
using N2.Security;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace N2.Tests.Content
{
    [TestFixture]
    public class ContentItemTests : ItemTestsBase
    {
        [Test]
        public void SettingValueType_AddsDetail()
        {
            AnItem item = new AnItem();
            item.IntProperty = 3;
            Assert.AreEqual(1, item.Details.Count);
            Assert.AreEqual(3, item.IntProperty);
            Assert.AreEqual(3, item.Details["IntProperty"].Value);
            Assert.AreEqual(3, item["IntProperty"]);
        }

        [Test]
        public void SettingValueTypeToDefailtRemovesDetail()
        {
            AnItem item = new AnItem();
            item.IntProperty = 3;
            item.IntProperty = 0;
            Assert.AreEqual(0, item.Details.Count);
        }

        [Test]
        public void SettingReferenceTypeAddsDetail()
        {
            AnItem item = new AnItem();
            item.StringProperty = "hello";
            Assert.AreEqual(1, item.Details.Count);
            Assert.AreEqual("hello", item.StringProperty);
            Assert.AreEqual("hello", item.Details["StringProperty"].Value);
            Assert.AreEqual("hello", item["StringProperty"]);
        }

        [Test]
        public void SettingReferenceTypeToDefailtRemovesDetail()
        {
            AnItem item = new AnItem();
            item.StringProperty = "hello";
            item.StringProperty = string.Empty;
            Assert.AreEqual(0, item.Details.Count);
        }

        [Test]
        public void AddTo_UpdatesParentRelation()
        {
            AnItem parent = new AnItem();
            AnItem child = new AnItem();

            child.AddTo(parent);
            Assert.AreEqual(parent, child.Parent);
        }

        [Test]
        public void AddTo_IsAddedToChildren()
        {
            AnItem parent = new AnItem();
            AnItem child = new AnItem();

            child.AddTo(parent);
            EnumerableAssert.Contains(parent.Children, child);
        }

        [Test]
        public void AddTo_IsRemovedFrom_PreviousParentChildren()
        {
            AnItem parent1 = new AnItem();
            AnItem parent2 = new AnItem();
            AnItem child = new AnItem();

            child.AddTo(parent1);
            EnumerableAssert.Contains(parent1.Children, child);
            EnumerableAssert.DoesntContain(parent2.Children, child);

            child.AddTo(parent2);
            EnumerableAssert.DoesntContain(parent1.Children, child);
            EnumerableAssert.Contains(parent2.Children, child);
        }

        [Test]
        public void AddTo_CanBeAddedToNull()
        {
            AnItem parent = new AnItem();
            AnItem child = new AnItem();

            child.AddTo(parent);
            Assert.AreEqual(parent, child.Parent);
            EnumerableAssert.Contains(parent.Children, child);

            child.AddTo(null);
            Assert.IsNull(child.Parent);
            EnumerableAssert.DoesntContain(parent.Children, child);
        }

        [Test]
        public void AddsToChildrenWhenOnlyParentIsSet()
        {
            AnItem parent = new AnItem();
            AnItem child = new AnItem();

            child.Parent = parent;

            child.AddTo(parent);
            EnumerableAssert.Contains(parent.Children, child);
        }

        [Test]
        public void AddTo_IsAppendedLast()
        {
            AnItem parent = new AnItem();
            AnItem child1 = new AnItem();
            AnItem child2 = new AnItem();
            AnItem child3 = new AnItem();
            AnItem child4 = new AnItem();

            child1.AddTo(parent);
            child2.AddTo(parent);
            child3.AddTo(parent);
            child4.AddTo(parent);

            Assert.AreEqual(child1, parent.Children[0]);
            Assert.AreEqual(child2, parent.Children[1]);
            Assert.AreEqual(child3, parent.Children[2]);
            Assert.AreEqual(child4, parent.Children[3]);
        }

        [Test]
        public void AddTo_IsAppendedLast_EvenWhenSortOrder_MayIndicateOtherwise()
        {
            AnItem parent = new AnItem();
            AnItem child1 = new AnItem();
            child1.SortOrder = 4;
            AnItem child2 = new AnItem();
            child2.SortOrder = 3;
            AnItem child3 = new AnItem();
            child3.SortOrder = 2;
            AnItem child4 = new AnItem();
            child4.SortOrder = 1;

            child1.AddTo(parent);
            child2.AddTo(parent);
            child3.AddTo(parent);
            child4.AddTo(parent);

            Assert.AreEqual(child1, parent.Children[0]);
            Assert.AreEqual(child2, parent.Children[1]);
            Assert.AreEqual(child3, parent.Children[2]);
            Assert.AreEqual(child4, parent.Children[3]);
        }

        [Test]
        public void DoesntAddToChildrenTwice()
        {
            AnItem parent = new AnItem();
            AnItem child = new AnItem();

            parent.Children.Add(child);

            child.AddTo(parent);
            Assert.AreEqual(parent, child.Parent);
            Assert.AreEqual(1, parent.Children.Count);
        }

        [Test]
        public void GetChild()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);

            Assert.AreEqual(item1, root.GetChild("item1"));
        }

        [Test]
        public void GetChild_NoItemYeldsNull()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);

            Assert.IsNull(root.GetChild("item2"));
        }

        [Test]
        public void GetChild_WithNullDoesntThrowException()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);

            Assert.IsNull(root.GetChild(null));
        }

        [Test]
        public void GetChild_WithEmptyStringDoesntThrowException()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);

            Assert.IsNull(root.GetChild(string.Empty));
        }

        [Test]
        public void GetChild_WithManyChildren()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            AnItem item2 = CreateOneItem<AnItem>(3, "item2", root);
            AnItem item3 = CreateOneItem<AnItem>(4, "item3", root);

            Assert.AreEqual(item1, root.GetChild("item1"));
            Assert.AreEqual(item2, root.GetChild("item2"));
            Assert.AreEqual(item3, root.GetChild("item3"));
        }

        [Test]
        public void GetChild_NameIncludingSlash_IsFound()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);

            Assert.That(root.GetChild("item1/"), Is.EqualTo(item1));
        }

        [Test]
        public void GetChild_NameIncludingDot()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item.1", root);

            Assert.AreEqual(item1, root.GetChild("item.1"));
        }

        [Test]
        public void GetChild_NameIncluding_Slash_IsFound()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item.1", root);

            Assert.That(root.GetChild("item.1/"), Is.EqualTo(item1));
        }

        [Test]
        public void GetChild_WhenName_IncludesUnicodeCharacter()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "ännu en ångande ö", root);

            Assert.AreEqual(item1, root.GetChild("ännu en ångande ö"));
        }

        [Test]
        public void GetChildsChild()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            AnItem item2 = CreateOneItem<AnItem>(2, "item2", item1);

            Assert.AreEqual(item2, root.GetChild("item1/item2"));
        }

        [Test]
        public void FindsGrandChild_WhenTrailingAspx()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            AnItem item2 = CreateOneItem<AnItem>(2, "item2", item1);

            Assert.That(root.GetChild("item1/item2/"), Is.EqualTo(item2));
        }

        [Test]
        public void GetChildsChildWithTrailContainingDots()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item.1", root);
            AnItem item2 = CreateOneItem<AnItem>(2, "item2", item1);

            Assert.AreEqual(item2, root.GetChild("item.1/item2"));
        }

        [Test]
        public void GetAncestorWayDown()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item", root);
            AnItem item2 = CreateOneItem<AnItem>(3, "item", item1);
            AnItem item3 = CreateOneItem<AnItem>(4, "item", item2);
            AnItem item4 = CreateOneItem<AnItem>(5, "item", item3);
            AnItem item5 = CreateOneItem<AnItem>(6, "item", item4);
            AnItem item6 = CreateOneItem<AnItem>(7, "item", item5);
            AnItem item7 = CreateOneItem<AnItem>(8, "item", item6);
            AnItem item8 = CreateOneItem<AnItem>(9, "item", item7);

            Assert.AreEqual(item2, root.GetChild("item/item"));
            Assert.AreEqual(item2, root.GetChild("item").GetChild("item"));
            Assert.AreEqual(item4, root.GetChild("item/item/item/item"));
            Assert.AreEqual(item4, root.GetChild("item/item").GetChild("item/item"));
            Assert.AreEqual(item8, root.GetChild("item/item/item/item").GetChild("item/item/item/item"));
            Assert.AreEqual(item8, root.GetChild("item/item/item/item/item/item/item/item"));
        }

        [Test]
        public void GetChild_HolesInPathYeldsNull()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            AnItem item2 = CreateOneItem<AnItem>(3, "item2", item1);
            AnItem item3 = CreateOneItem<AnItem>(4, "item3", item2);

            Assert.IsNull(root.GetChild("item1/itemX/item3"));
        }

        [Test]
        public void GetChild_CanFindCurrentItem()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            AnItem item2 = CreateOneItem<AnItem>(3, "item2", item1);

            Assert.AreEqual(root.GetChild("/"), root);
        }

        [Test]
        public void GetChild_CanFindItem_WhenTrailingSlash()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            AnItem item2 = CreateOneItem<AnItem>(3, "item2", item1);

            Assert.AreEqual(root.GetChild("/item1/"), item1);
        }

        [Test]
        public void GetChild_FindGrandChild_WhenTrailingSlash()
        {
            AnItem root = CreateOneItem<AnItem>(1, "root", null);
            AnItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            AnItem item2 = CreateOneItem<AnItem>(3, "item2", item1);

            Assert.AreEqual(root.GetChild("item1/item2/"), item2);
        }

        [Test]
        public void CanCloneItem()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            ContentItem clonedRoot = root.Clone(false);

            Assert.AreEqual(0, clonedRoot.ID);
            Assert.AreEqual(root.Name, clonedRoot.Name);
            Assert.AreEqual(root.Title, clonedRoot.Title);
        }

        [Test]
        public void CloningItem_WithNameSetToID_ClearsName()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "1", null);
            ContentItem clone = root.Clone(false);

            Assert.That(clone.Name, Is.Null);
        }

        [Test]
        public void CanCloneItem_WithProtectedDefaultConstructor()
        {
            ContentItem root = CreateOneItem<AnItemWithProtectedDefaultConstructor>(1, "root", null);
            ContentItem clonedRoot = root.Clone(false);

            Assert.AreEqual(0, clonedRoot.ID);
            Assert.AreEqual(root.Name, clonedRoot.Name);
            Assert.AreEqual(root.Title, clonedRoot.Title);
        }

		[Test]
		public void Clone_Equality()
		{
			var original = CreateItem<AnItem>("parent", null);
			original.ID = 1234;
			var referenced = CreateItem<AnItem>("parent", null);
			referenced.ID = 2345;

			original.Title = "N2 CMS Templates example";
			original["Text"] = @"<p>Welcome to the templates project. This site features a number of functions built with the N2 CMS framework. Use as is and/or build your own.</p>
<p>The most commonly used features are added below the feature page.</p>
<p>Don't forget to log into the administrative interface and experiment how content is edited. If you edit the start page you can try some other themes as well.</p>";
			original["Number"] = 123567;
			original["Decimal"] = 12345.123456;
			original["Guid"] = Guid.NewGuid();
			original["Boolean"] = true;
			original["Date"] = DateTime.Now;
			original["Timespan"] = TimeSpan.FromSeconds(12.34);
			original["Link"] = referenced;
			original.DetailCollections["Hello"].Add("World");


			var clone = original.Clone(includeChildren: false, includeIdentifier: true);

			
			var ignoredProperties = new HashSet<string> { "WritableGuid", "ReadOnlyGuid", "Details", "DetailCollections", "Item" };
			foreach (var pi in original.GetContentType().GetProperties().Where(p => !ignoredProperties.Contains(p.Name)))
				clone[pi.Name].ShouldBe(original[pi.Name]);

			clone.GetContentType().ShouldBe(original.GetContentType());
			clone.Details.Count.ShouldBe(original.Details.Count);
			foreach (var d in original.Details)
				clone.Details[d.Name].Value.ShouldBe(d.Value);
			clone.DetailCollections.Count.ShouldBe(original.DetailCollections.Count);
			foreach (var dc in original.DetailCollections)
				clone.DetailCollections[dc.Name].FirstOrDefault().ShouldBe(dc.FirstOrDefault());
		}

        [Test]
        public void CanSetDetail_ByIndexer()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            root["TheDetail"] = 1;

            Assert.That(root["TheDetail"], Is.EqualTo(1));
        }

        [Test]
        public void CanChange_DetailType()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            root["TheDetail"] = 1;
            root["TheDetail"] = "string";

            Assert.That(root["TheDetail"], Is.EqualTo("string"));
        }

        [TestCase("TheValue")]
        [TestCase(456)]
        [TestCase(456.567)]
        [TestCase(true)]
        [TestCase(false)]
        public void CanClone_Item_WithDetail(object value)
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            root["TheDetail"] = value;
            ContentItem clonedRoot = root.Clone(false);

            Assert.AreEqual(value, clonedRoot["TheDetail"], "The value " + value + " was not intact on the cloned item.");
        }

        [Test]
        public void CanClone_Item_WithDateTimeDetail()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            root["TheDetail"] = new DateTime(2009, 05, 09);
            ContentItem clonedRoot = root.Clone(false);

            Assert.AreEqual(new DateTime(2009, 05, 09), clonedRoot["TheDetail"]);
        }

        [Test]
        public void CanClone_Item_WithObjectDetail()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            X originalValue = new X { Number = 123};
            root["TheDetail"] = originalValue;
            ContentItem clonedRoot = root.Clone(false);

            X clonedValue = (X) clonedRoot["TheDetail"];
            Assert.AreEqual(clonedValue.Number, originalValue.Number);
        }

        [Test]
        public void CanClone_Item_WithDetailCollection()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            root.GetDetailCollection("TheDetailCollection", true).Add("TheValue");
            ContentItem clonedRoot = root.Clone(false);

            Assert.AreEqual("TheValue", clonedRoot.GetDetailCollection("TheDetailCollection", false)[0]);
        }

        [Test]
        public void AddTo_IsNoLongerInsertedBefore_ItemWithHighSortOrder()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            ContentItem child1 = CreateOneItem<AnItem>(2, "child1", root);
            child1.SortOrder = 1000000;

            ContentItem child2 = CreateOneItem<AnItem>(3, "child2", root);
            Assert.That(root.Children[1], Is.EqualTo(child2), "Should no longer be first because of child1's high sort order");
        }

        [Test]
        public void AddTo_ShouldBeLast_IfTreshold_IsNotMet()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            ContentItem child1 = CreateOneItem<AnItem>(0, "child1", root);
            ContentItem child2 = CreateOneItem<AnItem>(0, "child2", root);
            ContentItem child3 = CreateOneItem<AnItem>(0, "child3", root);
            child1.SortOrder = 9000;
            child2.SortOrder = 18000;
            child3.SortOrder = 27000;

            ContentItem child4 = CreateOneItem<AnItem>(0, "child4", root);
            Assert.That(root.Children[3], Is.EqualTo(child4), "Should be last since treshold is not met");
        }

        [Test]
        public void Updates_VersionedFields()
        {
            ContentItem source = CreateOneItem<AnItem>(1, "source", null);
            source.Created = new DateTime(2000, 1, 1);
            source.SavedBy = "someone";
            source.Title = "title";
            source.Updated = new DateTime(2010, 01, 01);
            source.Visible = false;
            
            ContentItem destination = CreateOneItem<AnItem>(2, "destination", null);
            ((N2.Persistence.IUpdatable<ContentItem>)destination).UpdateFrom(source);

            Assert.That(destination.Created, Is.EqualTo(new DateTime(2000, 1, 1)));
            Assert.That(destination.SavedBy, Is.EqualTo("someone"));
            Assert.That(destination.Title, Is.EqualTo("title"));
            Assert.That(destination.Updated, Is.EqualTo(new DateTime(2010, 01, 01)));
            Assert.That(destination.Visible, Is.False);
        }

        [Test]
        public void DoesntUpdate_UnversionedFields()
        {
            ContentItem source = CreateOneItem<AnItem>(1, "source", null);
            source.Expires = new DateTime(3000, 1, 1);
            source.Parent = new AnItem { Name = "parent" };
            source.Published = new DateTime(2005, 1, 1);
            source.SortOrder = 123;
            source.VersionOf = new AnItem { Name = "version" };
            source.ZoneName = "zone";

            ContentItem destination = CreateOneItem<AnItem>(2, "destination", null);
            ((N2.Persistence.IUpdatable<ContentItem>)destination).UpdateFrom(source);

            Assert.That(destination.Expires, Is.Not.EqualTo(new DateTime(3000, 1, 1)), "Expires should not be modified");
            Assert.That(destination.Parent, Is.Null, "Parent should not be modified");
            Assert.That(destination.Published, Is.Not.EqualTo(new DateTime(2005, 1, 1)), "Published should not be modified");
            Assert.That(destination.VersionOf.HasValue, Is.False, "VersionOf should not be modified");
        }

        [Test]
        public void DoesntUpdate_AuthorizedRoles()
        {
            ContentItem source = CreateOneItem<AnItem>(1, "source", null);
            source.AuthorizedRoles.Add(new AuthorizedRole(source, "Users"));

            ContentItem destination = CreateOneItem<AnItem>(2, "destination", null);
            ((N2.Persistence.IUpdatable<ContentItem>)destination).UpdateFrom(source);

            Assert.That(destination.AuthorizedRoles.Count, Is.EqualTo(0), "Roles should not be modified");
        }

        [Test]
        public void DoesntUpdate_Children()
        {
            ContentItem source = CreateOneItem<AnItem>(1, "source", null);
            source.Children.Add(new AnItem { Name = "child" });

            ContentItem destination = CreateOneItem<AnItem>(2, "destination", null);
            ((N2.Persistence.IUpdatable<ContentItem>)destination).UpdateFrom(source);

            Assert.That(destination.Children.Count, Is.EqualTo(0), "Children should not be modified");
        }

        [Test]
        public void Updates_ItemDetails()
        {
            ContentItem source = CreateOneItem<AnItem>(1, "source", null);
            source["Hello"] = "World";
            source.GetDetailCollection("World", true).Add("Hello");

            ContentItem destination = CreateOneItem<AnItem>(2, "destination", null);
            ((N2.Persistence.IUpdatable<ContentItem>)destination).UpdateFrom(source);

            Assert.That(destination["Hello"], Is.EqualTo("World"));
            Assert.That(destination.GetDetailCollection("World", false), Is.Not.Null);
            Assert.That(destination.GetDetailCollection("World", false).Count, Is.EqualTo(1));
            Assert.That(destination.GetDetailCollection("World", false)[0], Is.EqualTo("Hello"));
        }

        [Test]
        public void Clears_ItemDetails_NotInSourceVersion()
        {
            ContentItem source = CreateOneItem<AnItem>(1, "source", null);
            source.GetDetailCollection("Partial", true).Add(1);

            ContentItem destination = CreateOneItem<AnItem>(2, "destination", null);
            destination["Hello"] = "World";
            destination.GetDetailCollection("World", true).Add("Hello");
            destination.GetDetailCollection("Partial", true).Add(2);
            ((N2.Persistence.IUpdatable<ContentItem>)destination).UpdateFrom(source);

            Assert.That(destination["Hello"], Is.Null);
            Assert.That(destination.GetDetailCollection("World", false), Is.Null);
            Assert.That(destination.GetDetailCollection("Partial", false), Is.Not.Null);
            Assert.That(destination.GetDetailCollection("Partial", false).Count, Is.EqualTo(1));
            Assert.That(destination.GetDetailCollection("Partial", false)[0], Is.EqualTo(1));
        }

        [Test]
        public void SetDetail_ToString_AddsDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", "hello");

            Assert.That(item.Details["ADetail"], Is.Not.Null);
        }

        [Test]
        public void SetDetail_ToString_AddsStringDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", "hello");

            Assert.That(item.Details["ADetail"].ValueType, Is.EqualTo(typeof(string)));
            Assert.That(item.Details["ADetail"].ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.StringType));
        }

        [Test]
        public void SetDetail_ToString_AddsStringDetail_WithSameValue()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", "hello");

            Assert.That(item.Details["ADetail"].Value, Is.EqualTo("hello"));
        }

        [Test]
        public void SetDetail_ToString_WithDefault_AddsDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", "hello", "howdy");

            Assert.That(item.Details["ADetail"], Is.Not.Null);
        }

        [Test]
        public void SetDetail_ToString_WithDefault_AddsStringDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", "hello", "howdy");

            Assert.That(item.Details["ADetail"].ValueType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void SetDetail_ToString_WithDefault_AddsStringDetail_WithSameValue()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", "hello", "howdy");

            Assert.That(item.Details["ADetail"].Value, Is.EqualTo("hello"));
        }

        [Test]
        public void SetDetail_ToString_WithSameValueAsDefault_DoesntAddDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", "howdy", "howdy");

            Assert.That(item.Details.ContainsKey("ADetail"), Is.False);
        }

        [Test]
        public void SetDetail_ToBoolean_AddsDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", false);

            Assert.That(item.Details["ADetail"], Is.Not.Null);
        }

        [Test]
        public void SetDetail_ToBoolean_AddsBooleanDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", false);

            Assert.That(item.Details["ADetail"].ValueType, Is.EqualTo(typeof(bool)));
            Assert.That(item.Details["ADetail"].ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.BoolType));
        }

        [Test]
        public void SetDetail_ToBoolean_WithDefault_AddsDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", false, true);

            Assert.That(item.Details["ADetail"], Is.Not.Null);
        }

        [Test]
        public void SetDetail_ToBoolean_WithSameValueAsDefault_DoesntAddDetail_False()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", false, false);

            Assert.That(item.Details.ContainsKey("ADetail"), Is.False);
        }

        [Test]
        public void SetDetail_ToBoolean_WithSameValueAsDefault_DoesntAddDetail_True()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", true, true);

            Assert.That(item.Details.ContainsKey("ADetail"), Is.False);
        }

        [Test]
        public void SetDetail_WithBoolean_AddsDetail()
        {
            AnItem item = new AnItem();

            item.SetDetailAccessor("ADetail", false);

            Assert.That(item.Details["ADetail"], Is.Not.Null);
        }

        [Serializable]
        private class X
        {
            public int Number { get; set; }
        }
    }

    [TestFixture]
    public class ContentDetailTests
    {
        [Test]
        public void FactoryMethod_Bool()
        {
            var detail = ContentDetail.New(null, "Hello", true);

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.BoolValue, Is.True);
            Assert.That(detail.Value, Is.EqualTo(true));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(bool)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.BoolType));
        }

        [Test]
        public void FactoryMethod_DateTime()
        {
            var detail = ContentDetail.New(null, "Hello", new DateTime(2010, 06, 16));

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.DateTimeValue, Is.EqualTo(new DateTime(2010, 06, 16)));
            Assert.That(detail.Value, Is.EqualTo(new DateTime(2010, 06, 16)));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(DateTime)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.DateTimeType));
        }

        [Test]
        public void FactoryMethod_Double()
        {
            var detail = ContentDetail.New(null, "Hello", 123.456);

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.DoubleValue, Is.EqualTo(123.456));
            Assert.That(detail.Value, Is.EqualTo(123.456));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(double)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.DoubleType));
        }

        [Test]
        public void FactoryMethod_Int()
        {
            var detail = ContentDetail.New(null, "Hello", 234);

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.IntValue, Is.EqualTo(234));
            Assert.That(detail.Value, Is.EqualTo(234));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(int)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.IntType));
        }

        [Test]
        public void FactoryMethod_Link()
        {
            var item = new Definitions.Definitions.SideshowItem { ID = 123 };
            var detail = ContentDetail.New(null, "Hello", item);

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.LinkedItem, Is.EqualTo(item));
            Assert.That(detail.Value, Is.EqualTo(item));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(ContentItem)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.LinkType));
        }

        [Test]
        public void FactoryMethod_Object()
        {
            var detail = ContentDetail.New(null, "Hello", new[] { "World" });

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.Value, Is.InstanceOf<string[]>());
            Assert.That(((string[])detail.Value)[0], Is.EqualTo("World"));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(object)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.ObjectType));
        }

        [Test]
        public void FactoryMethod_String()
        {
            var detail = ContentDetail.New(null, "Hello", "World");

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.StringValue, Is.EqualTo("World"));
            Assert.That(detail.Value, Is.EqualTo("World"));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(string)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.StringType));
        }



        [Test]
        public void Constructor_Bool()
        {
            var detail = new ContentDetail(null, "Hello", true);

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.BoolValue, Is.True);
            Assert.That(detail.Value, Is.EqualTo(true));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(bool)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.BoolType));
        }

        [Test]
        public void Constructor_DateTime()
        {
            var detail = new ContentDetail(null, "Hello", new DateTime(2010, 06, 16));

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.DateTimeValue, Is.EqualTo(new DateTime(2010, 06, 16)));
            Assert.That(detail.Value, Is.EqualTo(new DateTime(2010, 06, 16)));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(DateTime)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.DateTimeType));
        }

        [Test]
        public void Constructor_Double()
        {
            var detail = new ContentDetail(null, "Hello", 123.456);

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.DoubleValue, Is.EqualTo(123.456));
            Assert.That(detail.Value, Is.EqualTo(123.456));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(double)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.DoubleType));
        }

        [Test]
        public void Constructor_Int()
        {
            var detail = new ContentDetail(null, "Hello", 234);

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.IntValue, Is.EqualTo(234));
            Assert.That(detail.Value, Is.EqualTo(234));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(int)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.IntType));
        }

        [Test]
        public void Constructor_Link()
        {
            var item = new Definitions.Definitions.SideshowItem { ID = 123 };
            var detail = new ContentDetail(null, "Hello", item);

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.LinkedItem, Is.EqualTo(item));
            Assert.That(detail.Value, Is.EqualTo(item));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(ContentItem)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.LinkType));
        }

        [Test]
        public void Constructor_Object()
        {
            var detail = new ContentDetail(null, "Hello", new[] { "World" });

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.Value, Is.InstanceOf<string[]>());
            Assert.That(((string[])detail.Value)[0], Is.EqualTo("World"));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(object)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.ObjectType));
        }

        [Test]
        public void Constructor_String()
        {
            var detail = new ContentDetail(null, "Hello", "World");

            Assert.That(detail, Is.InstanceOf<ContentDetail>());
            Assert.That(detail.StringValue, Is.EqualTo("World"));
            Assert.That(detail.Value, Is.EqualTo("World"));
            Assert.That(detail.ValueType, Is.EqualTo(typeof(string)));
            Assert.That(detail.ValueTypeKey, Is.EqualTo(ContentDetail.TypeKeys.StringType));
        }

        [Test]
        public void DetailCollection_String()
        {
            var dc = new DetailCollection();
            dc.Add("hello");

            dc.Details[0].ValueTypeKey.ShouldBe(ContentDetail.TypeKeys.StringType);
            dc[0].ShouldBe("hello");
        }

        [Test]
        public void DetailCollection_Int()
        {
            var dc = new DetailCollection();
            dc.Add(123);

            dc.Details[0].ValueTypeKey.ShouldBe(ContentDetail.TypeKeys.IntType);
            dc[0].ShouldBe(123);
        }

        [Test]
        public void DetailCollection_ContenItem()
        {
            var dc = new DetailCollection();
            dc.Add(new AnItem { ID = 123 });

            dc.Details[0].ValueTypeKey.ShouldBe(ContentDetail.TypeKeys.LinkType);
            ((ContentItem)dc[0]).ID.ShouldBe(123);
        }

        [Test]
        public void NoVersionOf_ComparedToNull()
        {
            var item = new AnItem();
            var version = new AnItem { ID = 2 };

            (item.VersionOf == null).ShouldBe(false); // no longer supported
            (item.VersionOf == version).ShouldBe(false);
        }

        [Test]
        public void NoVersionOf_InverseComparedToNull()
        {
            var item = new AnItem();
            var version = new AnItem { ID = 2 };

            (item.VersionOf != null).ShouldBe(true); // no longer supported
            (item.VersionOf != version).ShouldBe(true);
        }

        [Test]
        public void OneVersionOf_ComparedToNull()
        {
            var item = new AnItem();
            var version = new AnItem { ID = 2 };
            item.VersionOf = version;

            (item.VersionOf == null).ShouldBe(false);
            (item.VersionOf == version).ShouldBe(true);
        }

        [Test]
        public void OneVersionOf_InverseComparedToNull()
        {
            var item = new AnItem();
            var version = new AnItem { ID = 2 };
            item.VersionOf = version;

            (item.VersionOf != null).ShouldBe(true);
            (item.VersionOf != version).ShouldBe(false);
        }
    }
}
