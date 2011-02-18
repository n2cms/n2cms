using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Runtime;
using N2.Details;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public static class RegisterExtensions
	{
		public static ContentRegistration Define<T>(this ContentHelper<T> content, Action<ContentRegistration> registration = null) where T : class
		{
			var re = RegistrationExtensions.GetRegistrationExpression(content.Html);
			if (re != null)
			{
				re.GlobalSortOffset = 0;
				if (typeof(ContentItem).IsAssignableFrom(typeof(T)) && re.ContentType == null)
				{
					re.ContentType = typeof(T);
					re.Title = typeof(T).Name;
				}
				re.IsDefined = true;
				registration(re);
			}
			return re;
		}

		public static ContentRegistration AppendDefinition<T>(this ContentHelper<T> content, Action<ContentRegistration> registration = null) where T : class
		{
			var re = RegistrationExtensions.GetRegistrationExpression(content.Html);
			if (re != null)
			{
				re.GlobalSortOffset = 0;
				registration(re);
			}
			return re;
		}

		public static ContentRegistration PrependDefinition<T>(this ContentHelper<T> content, Action<ContentRegistration> registration = null) where T : class
		{
			var re = RegistrationExtensions.GetRegistrationExpression(content.Html);
			if (re != null)
			{
				re.GlobalSortOffset = -1000;
				registration(re);
			}
			return re;
		}

		//// containables

		public static ContentRegistration Tab(this ContentRegistration re, string containerName, string tabText, Action<ContentRegistration> registration, int? sortOrder = null)
		{
			if (re == null) return re;

			re.Add(new N2.Web.UI.TabContainerAttribute(containerName, tabText, re.NextSortOrder(sortOrder)));

			string previousContainerName = re.ContainerName;
			re.ContainerName = containerName;
			if (registration != null)
			{
				registration(re);
				re.ContainerName = previousContainerName;
			}

			return re;
		}

		public static ContentRegistration FieldSet(this ContentRegistration re, string containerName, string legend, Action<ContentRegistration> registration, int? sortOrder = null)
		{
			if (re == null) return re;

			string previousContainerName = re.ContainerName;
			re.ContainerName = containerName;
			re.Add(new N2.Web.UI.FieldSetContainerAttribute(containerName, legend, re.NextSortOrder(sortOrder)));

			if (registration != null)
			{
				registration(re);
				re.ContainerName = previousContainerName;
			}

			return re;
		}

		public static ContentRegistration Container(this ContentRegistration re, string name, Action<ContentRegistration> registration)
		{
			if (re == null) return re;

			string previousContainerName = re.ContainerName;
			re.ContainerName = name;
			if (registration != null)
			{
				registration(re);
				re.ContainerName = previousContainerName;
			}

			return re;
		}

		public static ContentRegistration EndContainer(this ContentRegistration re)
		{
			if (re == null) return re;

			re.ContainerName = null;
			return re;
		}

		//// editables

		//public static ContentRegistrationExpression Title(this ContentRegistrationExpression re, string title = "Title")
		//{
		//    if (re == null) return re;

		//    return re.Add(new WithEditableTitleAttribute(), title);
		//}

		//public static ContentRegistrationExpression Name(this ContentRegistrationExpression re, string title = "Name")
		//{
		//    if (re == null) return re;

		//    return re.Add(new WithEditableNameAttribute(), title);
		//}

		//public static ContentRegistrationExpression PublishedRange(this ContentRegistrationExpression re, string title = "Published between")
		//{
		//    if (re == null) return re;

		//    return re.Add(new WithEditablePublishedRangeAttribute(), title);
		//}

		//public static ContentRegistrationExpression DateRange(this ContentRegistrationExpression re, string nameStart, string nameEnd, string title = "Name")
		//{
		//    if (re == null) return re;

		//    return re.Add(new WithEditableDateRangeAttribute(title, 0, nameStart, nameEnd), title);
		//}

		//public static ContentRegistrationExpression CheckBox(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableCheckBoxAttribute(title ?? name, re.NextSortOrder(null)));
		//}

		//public static ContentRegistrationExpression Children(this ContentRegistrationExpression re, string zoneName, string name = null, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableChildrenAttribute(title ?? zoneName, zoneName, re.NextSortOrder(null)));
		//}

		//public static ContentRegistrationExpression Date(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableDateAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression Enum(this ContentRegistrationExpression re, string name, Type enumType, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableEnumAttribute(enumType), name, title);
		//}

		//public static ContentRegistrationExpression FileUpload(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableFileUploadAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression FreeTextArea(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableFreeTextAreaAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression Image(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableImageAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression ImageSize(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableImageSizeAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression Item(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableItemAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression LanguagesDropDown(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableLanguagesDropDownAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression Link(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableLinkAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression TextBox(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableTextBoxAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression ThemeSelection(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableThemeSelectionAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression Url(this ContentRegistrationExpression re, string name, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableUrlAttribute(), name, title);
		//}

		//public static ContentRegistrationExpression UserControl(this ContentRegistrationExpression re, string name, string userControlPath, string title = null)
		//{
		//    if (re == null) return re;

		//    return re.Add(new EditableUserControlAttribute(userControlPath, 0) { UserControlPath = userControlPath }, name, title);
		//}
	}

}