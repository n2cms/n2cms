using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2DevelopmentWeb.Customization;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2;

namespace N2DevelopmentWeb.Domain
{
	[Definition("My page", "PageItem", "This is the default item in the test project", "Click to use this type.", -10)]
	[RestrictParents(typeof (MyPageData))]
	[AllowedChildren(typeof (MyUnmappedItem2))]
	[AvailableZone("Right area", "Right")]
	[AvailableZone("Left area", "Left")]
	[AvailableZone("Content area", "Content")]
	[FieldSet("dropdowns", "Drop Down Lists", 240, ContainerName="links")]
	[Divider("hr", 30, ContainerName = "default")]
	[WithEditable("Hello", typeof (TextBox), "Text", 220, "Hello", ContainerName = "special")]
	[TabPanel("teasers", "Teasers", 1240)]
	[EditableChildren("Sub items", "Right", 0, ContainerName="teasers")]
	[WithEditablePublishedRange("Publish between", 20, ContainerName="default")]
	[TabPanel("advanced", "Advanced", 1040)]
	public class MyPageData : AbstractCustomItem
	{

		[N2.Details.Editable("Start date", typeof(DatePicker), "SelectedDate", 10, ContainerName = "advanced")]
		public virtual DateTime? MyProperty
		{
			get { return (DateTime?)(GetDetail("MyProperty")); }
			set { SetDetail("MyProperty", value); }
		}


		[EditableChildren("Left", "Left", 70, ContainerName = "special")]
		public virtual IList<ContentItem> Left
		{
			get { return GetChildren("Left"); }
		}
		[EditableChildren("Right", "Right", 80, ContainerName = "special")]
		public virtual IList<ContentItem> Right
		{
			get { return GetChildren("Right"); }
		}

		[EditableFreeTextArea("Text", 110, ContainerName="default")]
		public virtual string Text
		{
			get { return (string) GetDetail("Text"); }
			set { SetDetail("Text", value); }
		}

		[Editable("Right Zone Path", typeof (DropDownList), "SelectedValue", 99, DataBind = true, ContainerName="dropdowns")]
		[EditorModifier("DataSource", new string[] {"", ".", "/", ".."})]
		[EditorModifier("AutoPostBack", true)]
		public virtual string ZonePath
		{
			get { return (string) GetDetail("ZonePath") ?? ""; }
			set { SetDetail("ZonePath", value); }
		}

		[EditableUrl("Document or page url", 85, ContainerName="links")]
		public virtual string FileUrl
		{
			get { return (string) GetDetail("FileUrl"); }
			set { SetDetail("FileUrl", value); }
		}

		[Editable("My Item (id)", typeof (ItemSelector), "SelectedItemID", 80, ContainerName="links")]
		public virtual int MyItem
		{
			get { return (int) (GetDetail("MyItem") ?? 0); }
			set { SetDetail("MyItem", value); }
		}

		[EditableLink("My Item (reference)", 81, ContainerName = "links")]
		public virtual ContentItem MyItemRef
		{
			get { return (ContentItem)GetDetail("MyItemRef"); }
			set { SetDetail("MyItemRef", value); }
		}

		[Editable("My File", typeof (FileSelector), "Url", 90, ContainerName="links")]
		[N2.Serialization.FileAttachment]
		public virtual string MyFile
		{
			get { return (string) GetDetail("MyFile") ?? ""; }
			set { SetDetail("MyFile", value); }
		}

		[EditableTextBox("MyLabel", 100, ContainerName = "advanced")]
		public virtual string MyLabel
		{
			get { return (string)(GetDetail("MyLabel") ?? string.Empty); }
			set { SetDetail("MyLabel", value, string.Empty); }
		}


		public override string TemplateUrl
		{
			get { return AlternativeTemplateUrl; }
		}

		[Editable("Alternative Template Url", typeof (DropDownList), "SelectedValue", 99, DataBind = true,
			ContainerName = "dropdowns")]
		[EditorModifier("DataSource",
			new string[] {"~/default.aspx", "~/DSVTest.aspx", "~/AutomaticallyUpdatedTest.aspx", "~/FilterTest.aspx"})]
		public virtual string AlternativeTemplateUrl
		{
			get { return (string) GetDetail("AlternativeTemplateUrl") ?? "~/default.aspx"; }
			set { SetDetail("AlternativeTemplateUrl", value); }
		}

		[MyEditable(ContainerName="default")]
		public virtual string MyVerySpecialEditableProperty
		{
			get { return (string) (GetDetail("MyVerySpecialEditableProperty") ?? ""); }
			set { SetDetail("MyVerySpecialEditableProperty", value); }
		}

		[Editable("UnAllowedProperty", typeof (TextBox), "Text", 100)]
		[DetailAuthorizedRoles("NonExistantUserOrGroup")]
		public virtual string UnAllowedPropertey
		{
			get { return (string) (GetDetail("UnAllowedPropertey") ?? ""); }
			set { SetDetail("UnAllowedPropertey", value); }
		}

		[Editable("Integer value", typeof (TextBox), "Text", 100, ContainerName = "special")]
		public virtual int IntegerValue
		{
			get { return Convert.ToInt32(GetDetail("IntegerValue") ?? 0); }
			set { SetDetail("IntegerValue", value); }
		}
	}
}