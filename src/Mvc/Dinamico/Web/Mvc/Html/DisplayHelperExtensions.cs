using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;

namespace N2.Web.Mvc.Html
{
	public static class DisplayHelperExtensions
	{
		public static dynamic Title<TModel>(this DisplayHelper<TModel> display, string title = "Title", string container = null, Action<WithEditableTitleAttribute> config = null)
		{
			return display.Editable("Title", title, container, config);
		}

		public static dynamic Name<TModel>(this DisplayHelper<TModel> display, string title = "Name", string container = null, Action<WithEditableNameAttribute> config = null)
		{
			return display.Editable("Name", title, container, config);
		}

		public static dynamic PublishedRange<TModel>(this DisplayHelper<TModel> display, string title = "Published between", string container = null, Action<WithEditablePublishedRangeAttribute> config = null)
		{
			return display.Editable("Published", title, container, config);
		}

		public static dynamic DateRange<TModel>(this DisplayHelper<TModel> display, string nameStart, string nameEnd, string title = "Dates", string container = null, Action<WithEditableDateRangeAttribute> config = null)
		{
			Action<WithEditableDateRangeAttribute> config2;
			if (config != null)
				config2 = (a) => { a.NameEndRange = nameEnd; config(a); };
			else
				config2 = (a) => { a.NameEndRange = nameEnd; };

			return display.Editable(nameStart, title, container, config2);
		}

		//public static dynamic CheckBox<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableCheckBoxAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableCheckBoxAttribute(title ?? name, re.NextSortOrder(null)), config);
		//}

		//public static dynamic Children<TModel>(this DisplayHelper<TModel> display, string zoneName, string name = null, string title = null, Action<EditableChildrenAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableChildrenAttribute(title ?? zoneName, zoneName, re.NextSortOrder(null)), config);
		//}

		//public static dynamic Date<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableDateAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableDateAttribute(), name, title, config);
		//}

		//public static dynamic Enum<TModel>(this DisplayHelper<TModel> display, string name, Type enumType, string title = null, Action<EditableEnumAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableEnumAttribute(enumType), name, title, config);
		//}

		//public static dynamic FileUpload<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableFileUploadAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableFileUploadAttribute(), name, title, config);
		//}

		public static dynamic FreeText<TModel>(this DisplayHelper<TModel> display, string name, string title = null, string container = null, Action<EditableFreeTextAreaAttribute> config = null)
		{
			return display.Editable(name, title, container, config);
		}

		//public static dynamic Image<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableImageAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableImageAttribute(), name, title, config);
		//}

		//public static dynamic ImageSize<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableImageSizeAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableImageSizeAttribute(), name, title, config);
		//}

		//public static dynamic Item<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableItemAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableItemAttribute(), name, title, config);
		//}

		//public static dynamic LanguagesDropDown<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableLanguagesDropDownAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableLanguagesDropDownAttribute(), name, title, config);
		//}

		//public static dynamic Link<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableLinkAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableLinkAttribute(), name, title, config);
		//}

		public static dynamic Text<TModel>(this DisplayHelper<TModel> display, string name, string title = null, string container = null, Action<EditableTextBoxAttribute> config = null)
		{
			return display.Editable(name, title, container, config);
		}

		//public static dynamic ThemeSelection<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableThemeSelectionAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableThemeSelectionAttribute(), name, title, config);
		//}

		//public static dynamic Url<TModel>(this DisplayHelper<TModel> display, string name, string title = null, Action<EditableUrlAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableUrlAttribute(), name, title, config);
		//}

		//public static dynamic UserControl<TModel>(this DisplayHelper<TModel> display, string name, string userControlPath, string title = null, Action<EditableUserControlAttribute> config = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableUserControlAttribute(userControlPath, 0) { UserControlPath = userControlPath }, name, title, config);
		//}
	}

}