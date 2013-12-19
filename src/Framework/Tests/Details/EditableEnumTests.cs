using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using NUnit.Framework;
using N2.Tests.Details.Models;

namespace N2.Tests.Details
{
    [TestFixture]
    public class EditableEnumTests
    {
        private Page testBench;
        EditableEnumAttribute intEditable;
        EditableEnumAttribute stringEditable;
        EditableEnumAttribute enumEditable;

        private DropDownList intEditor;
        private DropDownList stringEditor;
        private DropDownList enumEditor;

        private EnumableItem item;

        [SetUp]
        public void SetUp()
        {
            testBench = new Page();
            intEditable = new EditableEnumAttribute("Integer", 0, typeof(enumDays)) { Name = "DaysInteger" };
            stringEditable = new EditableEnumAttribute("String", 1, typeof(enumDays)) { Name = "DaysString" };
            enumEditable = new EditableEnumAttribute("Enum", 2, typeof(enumDays)) { Name = "DaysEnum" };

            intEditor = (DropDownList)intEditable.AddTo(testBench);
            stringEditor = (DropDownList)stringEditable.AddTo(testBench);
            enumEditor = (DropDownList)enumEditable.AddTo(testBench);

            item = new EnumableItem();
            item.DaysEnum = enumDays.Mon;
            item.DaysInteger = 1;
            item.DaysString = "Mon";
        }

        [Test]
        public void CanCreate_Editor()
        {
            Assert.That(intEditor.Items.Count, Is.EqualTo(7));
            Assert.That(intEditor.Items[0].Value, Is.EqualTo("1"));
            Assert.That(intEditor.Items[6].Value, Is.EqualTo("7"));

            Assert.That(stringEditor.Items.Count, Is.EqualTo(7));
            Assert.That(stringEditor.Items[0].Value, Is.EqualTo("1"));
            Assert.That(stringEditor.Items[6].Value, Is.EqualTo("7"));

            Assert.That(enumEditor.Items.Count, Is.EqualTo(7));
            Assert.That(enumEditor.Items[0].Value, Is.EqualTo("1"));
            Assert.That(enumEditor.Items[6].Value, Is.EqualTo("7"));
        }

        [Test]
        public void Enum_CanWorkToString()
        {
            Assert.That(enumDays.Fri.ToString(), Is.EqualTo("Fri"));
        }

        [Test]
        public void Enum_CanBeCast_ToInteger()
        {
            Assert.That((int)enumDays.Fri, Is.EqualTo(5));
        }

        // can update the editor control from the property

        [Test]
        public void CanUpdate_IntEditor()
        {
            item.DaysInteger = 5;

            intEditable.UpdateEditor(item, intEditor);

            Assert.That(intEditor.SelectedValue, Is.EqualTo("5"));
        }

        [Test]
        public void CanUpdate_StringEditor()
        {
            item.DaysString = "Fri";

            stringEditable.UpdateEditor(item, stringEditor);

            Assert.That(stringEditor.SelectedValue, Is.EqualTo("5"));
        }

        [Test]
        public void CanUpdate_EnumEditor()
        {
            item.DaysEnum = enumDays.Fri;

            enumEditable.UpdateEditor(item, enumEditor);

            Assert.That(enumEditor.SelectedValue, Is.EqualTo("5"));
        }

        // can update the item based on values in the drop down list

        [Test]
        public void CanUpdate_IntProperty()
        {
            intEditor.SelectedValue = "5";

            intEditable.UpdateItem(item, intEditor);

            Assert.That(item.DaysInteger, Is.EqualTo(5));
        }

        [Test]
        public void CanUpdate_StringProperty()
        {
            stringEditor.SelectedValue = "5";

            stringEditable.UpdateItem(item, stringEditor);

            Assert.That(item.DaysString, Is.EqualTo("Fri"));
        }

        [Test]
        public void CanUpdate_EnumProperty()
        {
            enumEditor.SelectedValue = "5";

            enumEditable.UpdateItem(item, enumEditor);

            Assert.That(item.DaysEnum, Is.EqualTo(enumDays.Fri));
        }

        // can set values to same value twice (discussions/278316)

        [Test]
        public void CanUpdate_IntProperty_WhenAlreadyThatValue()
        {
            intEditor.SelectedValue = "5";
            enumEditable.UpdateItem(item, enumEditor);

            intEditable.UpdateItem(item, intEditor);

            Assert.That(item.DaysInteger, Is.EqualTo(5));
        }

        [Test]
        public void CanUpdate_StringProperty_WhenAlreadyThatValue()
        {
            stringEditor.SelectedValue = "5";
            enumEditable.UpdateItem(item, enumEditor);

            stringEditable.UpdateItem(item, stringEditor);

            Assert.That(item.DaysString, Is.EqualTo("Fri"));
        }

        [Test]
        public void CanUpdate_EnumProperty_WhenAlreadyThatValue()
        {
            enumEditor.SelectedValue = "5";
            enumEditable.UpdateItem(item, enumEditor);

            enumEditable.UpdateItem(item, enumEditor);

            Assert.That(item.DaysEnum, Is.EqualTo(enumDays.Fri));
        }
    }
}
