using System.Web.UI.WebControls;
using System.Linq;
using N2.Details;
using N2.Persistence;
using N2.Persistence.Proxying;
using NUnit.Framework;
using System.Collections.Generic;
using Shouldly;
using N2.Definitions.Static;

namespace N2.Tests.Persistence.Proxying
{
	#region Classes InterceptableItem, InterceptableInheritorItem, IgnoringItem
	public class InterceptableItem : ContentItem
	{
		[EditableCheckBox("My Property", 100)]
		public virtual bool BoolProperty { get; set; }

		[EditableEnum("My Property", 100, typeof(TextBoxMode))]
		public virtual TextBoxMode EnumProperty { get; set; }

		[EditableFreeTextArea("My Property", 100)]
		public virtual string StringProperty { get; set; }

		[EditableFreeTextArea("My String", 100, PersistAs = PropertyPersistenceLocation.DetailCollection, DefaultValue = new string[0])]
		public virtual IEnumerable<string> StringCollectionProperty { get; set; }

		[EditableFreeTextArea("My String", 100, PersistAs = PropertyPersistenceLocation.DetailCollection)]
		public virtual IEnumerable<ContentItem> LinkCollectionProperty { get; set; }

		[EditableFreeTextArea("My String", 100, PersistAs = PropertyPersistenceLocation.DetailCollection)]
		public virtual IEnumerable<InterceptableItem> TypedCollectionProperty { get; set; }

		[EditableFreeTextArea("My Numbers", 100, PersistAs = PropertyPersistenceLocation.DetailCollection)]
		public virtual int[] IntCollectionProperty { get; set; }

		[EditableItem("My Property", 100)]
		public virtual ContentItem LinkProperty { get; set; }

		public bool StandardBoolProperty_get = false;
		public bool StandardBoolProperty_set = false;
		[EditableCheckBox("My Property", 100)]
		public virtual bool StandardBoolProperty
		{
			get { StandardBoolProperty_get = true; return GetDetail("StandardBoolProperty", false); }
			set { StandardBoolProperty_set = true; SetDetail("StandardBoolProperty", value, false); }
		}
	}
	public class InterceptableInheritorItem : InterceptableItem
	{
	}

	public class IgnoringItem : InterceptableItem
	{
		[EditableCheckBox("Ignored Property", 100, PersistAs = PropertyPersistenceLocation.Ignore)]
		public virtual bool IgnoredProperty { get; set; }
	}
	#endregion

	[TestFixture]
	public class InterceptionFactoryTests
	{
		InterceptingProxyFactory factory;
		InterceptableItem item;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			factory = new InterceptingProxyFactory();
			var map = new DefinitionMap();
			factory.Initialize(new[] { typeof(InterceptableItem), typeof(InterceptableInheritorItem), typeof(IgnoringItem) }.Select(t => map.GetOrCreateDefinition(t)));
		}

		[SetUp]
		public void SetUp()
		{
			item = factory.Create(typeof(InterceptableItem).FullName, 0) as InterceptableItem;
		}

		// BOOL

		[Test]
		public void Get_BoolProperty()
		{
			item.SetDetail("BoolProperty", true, typeof(TextBoxMode));

			Assert.That(item.BoolProperty, Is.True);
		}

		[Test]
		public void Set_BoolProperty()
		{
			item.BoolProperty = true;

			Assert.That(item.GetDetail("BoolProperty"), Is.True);
		}

		[Test]
		public void Get_BoolProperty_DefaultValue()
		{
			Assert.That(item.BoolProperty, Is.False);
		}

		[Test]
		public void Set_BoolProperty_DefaultValue()
		{
			item.BoolProperty = false;

			Assert.That(item.GetDetail("BoolProperty"), Is.Null);
		}

		// ENUM

		[Test]
		public void Get_EnumProperty()
		{
			item.SetDetail("EnumProperty", TextBoxMode.Password, typeof(TextBoxMode));

			Assert.That(item.EnumProperty, Is.EqualTo(TextBoxMode.Password));
		}

		[Test]
		public void Set_EnumProperty()
		{
			item.EnumProperty = TextBoxMode.Password;

			Assert.That(item.GetDetail("EnumProperty"), Is.EqualTo(TextBoxMode.Password));
		}

		[Test]
		public void Get_EnumProperty_DefaultValue()
		{
			Assert.That(item.EnumProperty, Is.EqualTo(TextBoxMode.SingleLine));
		}

		[Test]
		public void Set_EnumProperty_DefaultValue()
		{
			item.EnumProperty = TextBoxMode.SingleLine;

			Assert.That(item.GetDetail("EnumProperty"), Is.Null);
		}

		// STRING

		[Test]
		public void Get_StringProperty()
		{
			item.SetDetail("StringProperty", "Hello", typeof(string));

			Assert.That(item.StringProperty, Is.EqualTo("Hello"));
		}

		[Test]
		public void Set_StringProperty()
		{
			item.StringProperty = "World";

			Assert.That(item.GetDetail("StringProperty"), Is.EqualTo("World"));
		}

		[Test]
		public void Get_StringProperty_DefaultValue()
		{
			Assert.That(item.StringProperty, Is.EqualTo(""));
		}

		[Test]
		public void Set_StringProperty_DefaultValue()
		{
			item.StringProperty = "";

			Assert.That(item.GetDetail("StringProperty"), Is.Null);
		}

		// COLLECTIONS

		[Test]
		public void Setting_StringCollectionProperty_AssignesToDetailCollection()
		{
			item.StringCollectionProperty = new string[] { "hello", "world" };
			Assert.That(item.GetDetailCollection("StringCollectionProperty", false), Is.EquivalentTo(new string[] { "hello", "world" }));
		}

		[Test]
		public void Getting_StringCollectionProperty_RetrievesFromDetailCollection()
		{
			item.GetDetailCollection("StringCollectionProperty", true).AddRange(new string[] { "hello", "world" });
			Assert.That(item.StringCollectionProperty, Is.EquivalentTo(new string[] { "hello", "world" }));
		}

		[Test]
		public void Getting_StringCollectionProperty_YieldsDefaultValue()
		{
			Assert.That(item.StringCollectionProperty, Is.EquivalentTo(new string[0]));
		}

		[Test]
		public void Setting_StringCollectionProperty_ToDefaultValue_DoesntStoreValue()
		{
			item.StringCollectionProperty = new string[0];

			Assert.That(item.GetDetailCollection("StringCollectionProperty", false), Is.Null);
		}

		[Test]
		public void Setting_StringCollectionProperty_ToDefaultValue_ClearsStoredValue()
		{
			item.StringCollectionProperty = new string[] { "hello", "world" };
			item.StringCollectionProperty = new string[0];

			Assert.That(item.GetDetailCollection("StringCollectionProperty", false), Is.Null);
		}

		[Test]
		public void Changing_StringCollectionProperty_AffectsDetailCollection()
		{
			item.StringCollectionProperty = new string[] { "hello", "world" };
			item.StringCollectionProperty = new string[] { "world" };

			Assert.That(item.GetDetailCollection("StringCollectionProperty", false), Is.EquivalentTo(new string[] { "world" }));
		}

		[Test]
		public void Setting_StringCollectionProperty_ToNull_RemovesCollection()
		{
			item.StringCollectionProperty = new string[] { "hello", "world" };
			item.StringCollectionProperty = null;

			Assert.That(item.GetDetailCollection("StringCollectionProperty", false), Is.Null);
		}

		[Test]
		public void Getting_ArrayCollectionProperty_RetrievesFromDetailCollection()
		{
			item.GetDetailCollection("IntCollectionProperty", true).AddRange(new[] { 123, 234 });
			Assert.That(item.IntCollectionProperty, Is.EquivalentTo(new[] { 123, 234 }));
		}

		[Test]
		public void Getting_IntCollectionProperty_WithoutDefaultValue_YieldsNull()
		{
			var value = item.IntCollectionProperty;

			Assert.That(value, Is.Null);
		}

		[Test]
		public void Setting_IntCollectionProperty_AssignsDetailCollections()
		{
			item.IntCollectionProperty = new int[0];

			Assert.That(item.GetDetailCollection("IntCollectionProperty", false), Is.EquivalentTo(new int[0]));
		}

		// LINK

		[Test]
		public void Get_LinkProperty()
		{
			item.SetDetail("LinkProperty", new InterceptableItem { ID = 222 }, typeof(ContentItem));

			Assert.That(item.LinkProperty, Is.EqualTo(new InterceptableItem { ID = 222 }));
		}

		[Test]
		public void Set_LinkProperty()
		{
			item.LinkProperty = new InterceptableItem { ID = 666 };

			Assert.That(item.GetDetail("LinkProperty"), Is.EqualTo(new InterceptableItem { ID = 666 }));
		}

		[Test]
		public void Get_LinkProperty_DefaultValue()
		{
			Assert.That(item.LinkProperty, Is.Null);
		}

		[Test]
		public void Set_LinkProperty_DefaultValue()
		{
			item.LinkProperty = null;

			Assert.That(item.GetDetail("LinkProperty"), Is.Null);
		}

		// LINK COLLECTION

		[Test]
		public void Setting_LinkCollectionProperty_AssignesTo_DetailCollection()
		{
			var values = new ContentItem[] { new InterceptableItem { ID = 1 }, new InterceptableItem { ID = 2 } };
			item.LinkCollectionProperty = values;
			Assert.That(item.GetDetailCollection("LinkCollectionProperty", false), Is.EquivalentTo(values));
		}

		[Test]
		public void Setting_LinkCollectionProperty_AssignesTo_DetailCollection_typed()
		{
			var values = new InterceptableItem[] { new InterceptableItem { ID = 1 }, new InterceptableItem { ID = 2 } };
			item.TypedCollectionProperty = values;
			Assert.That(item.GetDetailCollection("TypedCollectionProperty", false), Is.EquivalentTo(values));
		}

		[Test]
		public void Getting_LinkCollectionProperty_ReadsFrom_DetailCollection()
		{
			var values = new ContentItem[] { new InterceptableItem { ID = 1 }, new InterceptableItem { ID = 2 } };
			item.GetDetailCollection("LinkCollectionProperty", true).AddRange(values);
			item.LinkCollectionProperty.ShouldBe(values);
		}

		[Test]
		public void Getting_LinkCollectionProperty_ReadsFrom_DetailCollection_typed()
		{
			var values = new InterceptableItem[] { new InterceptableItem { ID = 1 }, new InterceptableItem { ID = 2 } };
			item.GetDetailCollection("TypedCollectionProperty", true).AddRange(values);
			item.TypedCollectionProperty.ShouldBe(values);
		}

		// STANDARD

		[Test]
		public void Get_DoesntIntercept_StandardBoolProperty()
		{
			var value = item.StandardBoolProperty;

			Assert.That(item.StandardBoolProperty_get);
			Assert.That(item.StandardBoolProperty_get);
		}

		[Test]
		public void Set_DoesntIntercept_StandardBoolProperty()
		{
			item.StandardBoolProperty = true;

			Assert.That(item.StandardBoolProperty_set);
		}

		// GetEntityName

		[Test]
		public void GetEntityName_ReturnsOriginalDiscriminator()
		{
			string name = factory.GetTypeName(item);

			Assert.That(name, Is.EqualTo(typeof(InterceptableItem).FullName));
		}

		// GetContentType

		[Test]
		public void GetContentType_DoesntReturnProxyType()
		{
			var type = item.GetContentType();

			Assert.That(type, Is.EqualTo(typeof(InterceptableItem)));
		}

		// Inheritance

		[Test]
		public void InheritingClass_WithInterceptedProperties_Get_IsIntercepted()
		{
			var inheritor = factory.Create(typeof(InterceptableInheritorItem).FullName, 0) as InterceptableInheritorItem;
			inheritor.SetDetail("StringProperty", "hello", typeof(string));

			Assert.That(inheritor.StringProperty, Is.EqualTo("hello"));
		}

		[Test]
		public void InheritingClass_WithInterceptedProperties_Set_IsIntercepted()
		{
			var inheritor = factory.Create(typeof(InterceptableInheritorItem).FullName, 0) as InterceptableInheritorItem;
			inheritor.StringProperty = "world";

			Assert.That(inheritor.GetDetail("StringProperty"), Is.EqualTo("world"));
		}

		// Ignore properties

		[Test]
		public void IgnoringClass_WithIgnoredProperty_Get_IsNotIntercepted()
		{
			var ignoring = factory.Create(typeof(IgnoringItem).FullName, 0) as IgnoringItem;
			ignoring.SetDetail("IgnoredProperty", true, typeof(bool));

			Assert.That(ignoring.IgnoredProperty, Is.False);
		}

		[Test]
		public void IgnoringClass_WithIgnoredProperty_Set_IsNotIntercepted()
		{
			var ignoring = factory.Create(typeof(IgnoringItem).FullName, 0) as IgnoringItem;
			ignoring.IgnoredProperty = true;

			Assert.That(ignoring.GetDetail("IgnoredProperty"), Is.Null);
		}

		// Unproxied entities

		[Test]
		public void Unproxied_StoresChanges_OnSaving()
		{
			var interceptable = new InterceptableItem();

			interceptable.BoolProperty = true;
			interceptable.EnumProperty = TextBoxMode.Password;
			interceptable.StringProperty = "hello";

			factory.OnSaving(interceptable);

			Assert.That(interceptable.GetDetail("BoolProperty"), Is.EqualTo(true));
			Assert.That(interceptable.GetDetail("EnumProperty"), Is.EqualTo(TextBoxMode.Password));
			Assert.That(interceptable.GetDetail("StringProperty"), Is.EqualTo("hello"));
		}

		[Test]
		public void Unproxied_ReportsChanges_WhenChangesWereMade()
		{
			var interceptable = new InterceptableItem();

			interceptable.BoolProperty = true;

			bool wasChanged = factory.OnSaving(interceptable);

			Assert.That(wasChanged);
		}

		[Test]
		public void Unproxied_DoesntReportChanges_WhenNoChangesWereMade()
		{
			var interceptable = new InterceptableItem();
			interceptable.SetDetail("BoolProperty", false, typeof(bool));
			interceptable.SetDetail("EnumProperty", TextBoxMode.SingleLine, typeof(TextBoxMode));
			interceptable.SetDetail("StringProperty", null, typeof(string));

			bool wasChanged = factory.OnSaving(interceptable);

			Assert.That(wasChanged, Is.False);
		}

		[Test]
		public void Attributes_are_retieved_via_definition()
		{

		}

		//// Copy
		
		//[Test]
		//public void Copy()
		//{
		//    var item1 = factory.Create(typeof(InterceptableItem).FullName, 1) as ContentItem;
		//    var item2 = factory.Create(typeof(InterceptableItem).FullName, 2) as ContentItem;
		//    item2.AddTo(item1);
		//    var item3 = factory.Create(typeof(InterceptableItem).FullName, 3) as ContentItem;
		//    item3.AddTo(item2);

		//    var clone = item1.Clone(true);
		//    var tn1 = factory.GetTypeName(clone);
		//    var tn2 = factory.GetTypeName(clone.Children[0]);
		//    var tn3 = factory.GetTypeName(clone.Children[0].Children[0]);
		//}
	}
}
