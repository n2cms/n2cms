#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;
using N2.Web.UI;
using N2.Web.Mvc;
using N2.Definitions.Runtime;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
    [PageDefinition(SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
    public class TestPage : TestItemBase
    {
    }

    [PageDefinition(Title = "TestPage [multiple item selection]", SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
    public class TestPageWithEditableMultipleItemSelection : TestPage
    {
        [N2.Details.EditableMultipleItemSelection]
        public virtual IEnumerable<ContentItem> EditableMultipleItemSelection { get; set; }
    }

    [PageDefinition(Title = "TestPage [checkbox list]", SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
    public class TestPageWithEditableCheckBoxList : TestPage
    {
        [EditableCheckBoxList]
        public virtual IList<string> EditableCheckBoxList { get; set; }
    }

    [PageDefinition(Title = "TestPage [children]", SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
    public class TestPageWithEditableChildren : TestPage
    {
        [EditableChildren]
        public virtual IEnumerable<TestPart> EditableChildren { get; set; }
    }

    [PageDefinition(Title = "TestPage [child]", SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
    public class TestPageWithEditableItem : TestPage
    {
        [EditableItem(Title = "Editable Item")]
        public virtual TestPartWithText EditableItem { get; set; }
    }

    [PageDefinition(Title = "TestPage [child+children]", SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
    public class TestPageWithEditableItemAndChildren : TestPage
    {
        [EditableItem(Title = "Editable Item")]
        public virtual TestPartWithText EditableItem { get; set; }

        [EditableChildren]
        public virtual IEnumerable<TestPart> EditableChildren { get; set; }
    }

    [PageDefinition(Title = "TestPage [all]", SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
    [RestrictParents(typeof(StartPage))]
    [WithEditableTitle(Placeholder = "Title", DefaultValue = "Test"), WithEditableName(Placeholder = "Name", DefaultValue = "test"), WithEditableDateRange(FromDatePlaceholder = "From date", FromTimePlaceholder = "From time", ToDatePlaceholder = "To date", ToTimePlaceholder = "To time"), WithEditablePublishedRange(FromDatePlaceholder = "From date", FromTimePlaceholder = "From time", ToDatePlaceholder = "To date", ToTimePlaceholder = "To time"), WithEditableTemplateSelection, WithEditableVisibility]
    [TabContainer("content", "Test", 1000)] // container named content automatically receives all editors without container set
    [TabContainer("SubTestContainer", "SubTest", -1, ContainerName = "content")]
    [TabContainer("SubTestContainer2", "SubTest 2", -1, ContainerName = "content")]
    public class TestPageWithAll : TestPage
    {
        [EditableCheckBox(ContainerName = "SubTestContainer")]
        public virtual bool EditableCheckBox { get; set; }

        [EditableChildren]
        public virtual IEnumerable<TestPart> EditableChildren { get; set; }

        [EditableDate(Placeholder = "Date", TimePlaceholder = "Time")]
        public virtual string EditableDate { get; set; }

        [EditableDefinition]
        public virtual string EditableDefinition { get; set; }

        [EditableEnum(EnumType = typeof(Base64FormattingOptions))]
        public virtual Base64FormattingOptions EditableEnum { get; set; }

        [EditableFileUpload(Placeholder = "File upload")]
        public virtual string EditableFileUpload { get; set; }

        [EditableFolderSelection(Placeholder = "Folder", PreferredSize = "icon")]
        public virtual string EditableFolderSelection { get; set; }

        [EditableFreeTextArea]
        public virtual string EditableFreeTextArea { get; set; }

        [EditableImage(Placeholder = "Image")]
        public virtual string EditableImage { get; set; }

        [EditableImageSize(ContainerName = "SubTestContainer2")]
        public virtual string EditableImageSize { get; set; }

        [EditableImageUpload(Placeholder = "Image upload")]
        public virtual string EditableImageUpload { get; set; }

        [EditableItem(Title = "Editable Item")]
        public virtual TestPartWithText EditableItem { get; set; }

        [EditableLanguagesDropDown]
        public virtual string EditableLanguagesDropDown { get; set; }

        [EditableLink(Placeholder = "Link to item")]
        public virtual ContentItem EditableLink { get; set; }

        [EditableMediaUpload(Placeholder = "Media file")]
        public virtual string EditableMediaUpload { get; set; }

        [EditableMetaTag(Placeholder = "Meta text")]
        public virtual string EditableMetaTag { get; set; }

        [EditableNumber(Placeholder = "Number")]
        public virtual string EditableNumber { get; set; }

        [EditableSummary(Placeholder = "Summary text")]
        public virtual string EditableSummary { get; set; }

        [EditableTags(Placeholder = "Tags")]
        public virtual IEnumerable<string> EditableTags { get; set; }

        [EditableText(Placeholder = "Text")]
        public virtual string EditableText { get; set; }

        [EditableText(DefaultValue = "Default Value")]
        public virtual string DefaultValue { get; set; }

        [EditableThemeSelection]
        public virtual string EditableThemeSelection { get; set; }

        [EditableUrl(Placeholder = "Url")]
        public virtual string EditableUrl { get; set; }

        [EditableUserControl(UserControlPath = "~/Areas/Tests/Uc.ascx")]
        public virtual string EditableUserControl { get; set; }

        [N2.Details.EditableMultipleItemSelection]
        public virtual IEnumerable<ContentItem> EditableMultipleItemSelection { get; set; }

        [N2.Details.EditableItemSelection]
        public virtual ContentItem EditableItemSelection { get; set; }
    }

    public class EditableCheckBoxListAttribute : EditableListControlAttribute
    {
        public Type ProviderType { get; set; }

        protected override Control AddEditor(Control container)
        {
            var ddl = CreateEditor();
            ddl.ID = Name;
            ddl.Items.AddRange(GetListItems());
            container.Controls.Add(ddl);
            return ddl;
        }

        protected override ListControl CreateEditor()
        {
            return new CheckBoxList();
        }


        public override void UpdateEditor(ContentItem item, Control editor)
        {
            var listControl = (ListControl)editor;

            var list = item[Name] as IList<string> ?? new List<string>();

            foreach (ListItem listItem in listControl.Items)
            {
                if (list.Contains(listItem.Value))
                    listItem.Selected = true;
            }

            base.UpdateEditor(item, editor);

        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            var listControl = (ListControl)editor;

            if (item[Name] == null || item[Name] as IList == null)
            {
                item[Name] = new List<string>();
            }

            var list = (IList)item[Name];

            list.Clear();

            foreach (ListItem listItem in listControl.Items.Cast<ListItem>().Where(listItem => listItem.Selected))
            {
                list.Add(listItem.Value);
            }

            item[Name] = list;

            return true;
        }

        protected override ListItem[] GetListItems()
        {
            return Enumerable.Range(1, 10)
                .Select(i => new ListItem(i.ToString()))
                .ToArray();
        }
    }
}
#endif
