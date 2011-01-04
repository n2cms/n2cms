using System.Web.UI.WebControls;
using N2.Details;
using N2.Persistence;
using N2.Persistence.Proxying;
using NUnit.Framework;

namespace N2.Tests.Persistence.Proxying
{
	public class InterceptableItem : ContentItem
	{
		[EditableCheckBox("My Property", 100)]
		public virtual bool BoolProperty { get; set; }

		[EditableEnum("My Property", 100, typeof(TextBoxMode))]
		public virtual TextBoxMode EnumProperty { get; set; }

		[EditableFreeTextArea("My Property", 100)]
		public virtual string StringProperty { get; set; }

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

	[TestFixture]
	public class InterceptionFactoryTests
	{
		InterceptingProxyFactory factory;
		InterceptableItem item;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			factory = new InterceptingProxyFactory();
			factory.Initialize(new[] { typeof(InterceptableItem), typeof(InterceptableInheritorItem), typeof(IgnoringItem) });
		}

		[SetUp]
		public void SetUp()
		{
			item = factory.Create(typeof(InterceptableItem).FullName) as InterceptableItem;
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
			var inheritor = factory.Create(typeof(InterceptableInheritorItem).FullName) as InterceptableInheritorItem;
			inheritor.SetDetail("StringProperty", "hello", typeof(string));

			Assert.That(inheritor.StringProperty, Is.EqualTo("hello"));
		}

		[Test]
		public void InheritingClass_WithInterceptedProperties_Set_IsIntercepted()
		{
			var inheritor = factory.Create(typeof(InterceptableInheritorItem).FullName) as InterceptableInheritorItem;
			inheritor.StringProperty = "world";

			Assert.That(inheritor.GetDetail("StringProperty"), Is.EqualTo("world"));
		}

		// Ignore properties

		[Test]
		public void IgnoringClass_WithIgnoredProperty_Get_IsNotIntercepted()
		{
			var ignoring = factory.Create(typeof(IgnoringItem).FullName) as IgnoringItem;
			ignoring.SetDetail("IgnoredProperty", true, typeof(bool));

			Assert.That(ignoring.IgnoredProperty, Is.False);
		}

		[Test]
		public void IgnoringClass_WithIgnoredProperty_Set_IsNotIntercepted()
		{
			var ignoring = factory.Create(typeof(IgnoringItem).FullName) as IgnoringItem;
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
	}
}
