using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;

namespace N2.Web.Mvc.Html
{
	public static class DisplayHelperExtensions
	{
		public static EditorBuilder<WithEditableTitleAttribute> Title<TModel>(this DisplayHelper<TModel> display, string title = "Title", string container = null)
		{
			return display.Editable<WithEditableTitleAttribute>("Title", title, container);
		}

		public static EditorBuilder<WithEditableNameAttribute> Name<TModel>(this DisplayHelper<TModel> display, string title = "Name", string container = null)
		{
			return display.Editable<WithEditableNameAttribute>("Name", title, container);
		}

		public static EditorBuilder<WithEditablePublishedRangeAttribute> PublishedRange<TModel>(this DisplayHelper<TModel> display, string title = "Published between", string container = null)
		{
			return display.Editable<WithEditablePublishedRangeAttribute>("Published", title, container);
		}

		public static EditorBuilder<WithEditableDateRangeAttribute> DateRange<TModel>(this DisplayHelper<TModel> display, string nameStart, string nameEnd, string title = "Dates", string container = null)
		{
			return display.Editable<WithEditableDateRangeAttribute>(nameStart, title, container)
				.Configure(a => a.NameEndRange = nameEnd);
		}

		//public static dynamic CheckBox<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableCheckBoxAttribute(title ?? name, re.NextSortOrder(null)));
		//}

		//public static dynamic Children<TModel>(this DisplayHelper<TModel> display, string zoneName, string name = null, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableChildrenAttribute(title ?? zoneName, zoneName, re.NextSortOrder(null)));
		//}

		//public static dynamic Date<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableDateAttribute(), name, title);
		//}

		//public static dynamic Enum<TModel>(this DisplayHelper<TModel> display, string name, Type enumType, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableEnumAttribute(enumType), name, title);
		//}

		//public static dynamic FileUpload<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableFileUploadAttribute(), name, title);
		//}

		public static EditorBuilder<EditableFreeTextAreaAttribute> FreeText<TModel>(this DisplayHelper<TModel> display, string name, string title = null, string container = null)
		{
			return display.Editable<EditableFreeTextAreaAttribute>(name, title, container);
		}

		//public static dynamic Image<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableImageAttribute(), name, title);
		//}

		//public static dynamic ImageSize<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableImageSizeAttribute(), name, title);
		//}

		//public static dynamic Item<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableItemAttribute(), name, title);
		//}

		//public static dynamic LanguagesDropDown<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableLanguagesDropDownAttribute(), name, title);
		//}

		//public static dynamic Link<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableLinkAttribute(), name, title);
		//}

		public static EditorBuilder<EditableTextBoxAttribute> Text<TModel>(this DisplayHelper<TModel> display, string name, string title = null, string container = null)
		{
			return display.Editable<EditableTextBoxAttribute>(name, title, container);
		}

		//public static dynamic ThemeSelection<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableThemeSelectionAttribute(), name, title);
		//}

		//public static dynamic Url<TModel>(this DisplayHelper<TModel> display, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableUrlAttribute(), name, title);
		//}

		//public static dynamic UserControl<TModel>(this DisplayHelper<TModel> display, string name, string userControlPath, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableUserControlAttribute(userControlPath, 0) { UserControlPath = userControlPath }, name, title);
		//}
	}

}