using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using N2.Definitions.Runtime;

namespace N2.Web.Mvc.Html
{
	public static class EditableRegistrationExtensions
	{
		public static EditableBuilder<WithEditableTitleAttribute> Title(this IContentRegistration registration, string title = "Title")
		{
			return registration.RegisterEditable<WithEditableTitleAttribute>("Title", title);
		}

		public static EditableBuilder<WithEditableNameAttribute> Name(this IContentRegistration registration, string title = "Name")
		{
			return registration.RegisterEditable<WithEditableNameAttribute>("Name", title);
		}

		public static EditableBuilder<WithEditablePublishedRangeAttribute> PublishedRange(this IContentRegistration registration, string title = "Published between")
		{
			return registration.RegisterEditable<WithEditablePublishedRangeAttribute>("Published", title);
		}

		public static EditableBuilder<WithEditableDateRangeAttribute> DateRange(this IContentRegistration registration, string nameStart, string nameEnd, string title = "Dates")
		{
			return registration.RegisterEditable<WithEditableDateRangeAttribute>(nameStart, title)
				.Configure(a => a.NameEndRange = nameEnd);
		}

		public static EditableBuilder<EditableCheckBoxAttribute> CheckBox(this IContentRegistration registration, string name, string checkBoxText = null)
		{
			return registration.RegisterEditable<EditableCheckBoxAttribute>(name, "")
				.Configure(e => e.CheckBoxText = checkBoxText);
		}

		public static EditableBuilder<EditableChildrenAttribute> Children(this IContentRegistration registration, string zoneName)
		{
			return registration.RegisterEditable<EditableChildrenAttribute>(zoneName, zoneName);
		}

		public static EditableBuilder<EditableDateAttribute> Date(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableDateAttribute>(name, title);
		}

		public static EditableBuilder<EditableEnumAttribute> Enum(this IContentRegistration registration, string name, Type enumType, string title = null)
		{
			return registration.RegisterEditable<EditableEnumAttribute>(name, title)
				.Configure(e => e.EnumType = enumType);
		}

		public static EditableBuilder<EditableDropDownAttribute> DropDown(this IContentRegistration registration, string name, params System.Web.UI.WebControls.ListItem[] listItems)
		{
			return registration.RegisterEditable<EditableDropDownAttribute>(new CustomDropDownAttribute() { Name = name, Title = name })
				.Configure(e => ((CustomDropDownAttribute)e).ListItems = listItems);
		}

		class CustomDropDownAttribute : EditableDropDownAttribute
		{
			public System.Web.UI.WebControls.ListItem[] ListItems { get; set; }
			protected override System.Web.UI.WebControls.ListItem[] GetListItems()
			{
				return ListItems;
			}
		}


		public static EditableBuilder<EditableFileUploadAttribute> FileUpload(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableFileUploadAttribute>(name, title);
		}

		public static EditableBuilder<EditableFreeTextAreaAttribute> FreeText(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableFreeTextAreaAttribute>(name, title);
		}

		public static EditableBuilder<EditableImageAttribute> Image(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableImageAttribute>(name, title);
		}

		public static EditableBuilder<EditableImageSizeAttribute> ImageSize(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableImageSizeAttribute>(name, title);
		}

		public static EditableBuilder<EditableItemAttribute> Item(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableItemAttribute>(name, title);
		}

		public static EditableBuilder<EditableLanguagesDropDownAttribute> Languages(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableLanguagesDropDownAttribute>(name, title);
		}

		public static EditableBuilder<EditableLinkAttribute> Link(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableLinkAttribute>(name, title);
		}

		public static EditableBuilder<EditableTextAttribute> Text(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableTextAttribute>(name, title);
		}

		public static EditableBuilder<EditableMetaTagAttribute> Meta(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableMetaTagAttribute>(name, title);
		}

		public static EditableBuilder<EditableThemeSelectionAttribute> ThemeSelection(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableThemeSelectionAttribute>(name, title);
		}

		public static EditableBuilder<EditableUrlAttribute> Url(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableUrlAttribute>(name, title);
		}

		public static EditableBuilder<EditableUserControlAttribute> UserControl(this IContentRegistration registration, string name, string userControlPath, string title = null)
		{
			return registration.RegisterEditable<EditableUserControlAttribute>(name, title)
				.Configure(e => e.UserControlPath = userControlPath);
		}
	}

}