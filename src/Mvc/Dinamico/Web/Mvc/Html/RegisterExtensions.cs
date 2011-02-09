using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions.Dynamic;
using N2.Details;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public static class RegisterExtensions
	{
		public static DefinitionRegistrationExpression Define<T>(this ContentHelper<T> content, Action<DefinitionRegistrationExpression> registration = null) where T : class
		{
			var re = GetRegistrationExpression<T>(content.Html);
			if (re != null)
			{
				re.SortOffset = 0;
				if (typeof(ContentItem).IsAssignableFrom(typeof(T)) && re.ItemType == null)
				{
					re.ItemType = typeof(T);
					re.Title = typeof(T).Name;
				}
				re.IsDefined = true;
				registration(re);
			}
			return re;
		}

		public static DefinitionRegistrationExpression AppendDefinition<T>(this ContentHelper<T> content, Action<DefinitionRegistrationExpression> registration = null) where T : class
		{
			var re = GetRegistrationExpression<T>(content.Html);
			if (re != null)
			{
				re.SortOffset = 0;
				registration(re);
			}
			return re;
		}

		public static DefinitionRegistrationExpression PrependDefinition<T>(this ContentHelper<T> content, Action<DefinitionRegistrationExpression> registration = null) where T : class
		{
			var re = GetRegistrationExpression<T>(content.Html);
			if (re != null)
			{
				re.SortOffset = -1000;
				registration(re);
			}
			return re;
		}

		public static DefinitionRegistrationExpression GetRegistrationExpression<T>(HtmlHelper<T> html)
		{
			return html.ViewContext.ViewData["RegistrationExpression"] as DefinitionRegistrationExpression;
		}

		// containables

		public static DefinitionRegistrationExpression Tab(this DefinitionRegistrationExpression re, string containerName, string tabName, Action<DefinitionRegistrationExpression> registration = null, int? sortOrder = null)
		{
			if (re == null) return re;

			re.AddContainable(new N2.Web.UI.TabContainerAttribute(containerName, tabName, re.NextSortOrder(sortOrder)));

			string previousContainerName = re.ContainerName;
			re.ContainerName = containerName;
			if (registration != null)
			{
				registration(re);
				re.ContainerName = previousContainerName;
			}

			return re;
		}

		public static DefinitionRegistrationExpression FieldSet(this DefinitionRegistrationExpression re, string containerName, string legend, Action<DefinitionRegistrationExpression> registration = null, int? sortOrder = null)
		{
			if (re == null) return re;

			string previousContainerName = re.ContainerName;
			re.ContainerName = containerName;
			re.AddContainable(new N2.Web.UI.FieldSetContainerAttribute(containerName, legend, re.NextSortOrder(sortOrder)));

			if (registration != null)
			{
				registration(re);
				re.ContainerName = previousContainerName;
			}

			return re;
		}

		public static DefinitionRegistrationExpression Container(this DefinitionRegistrationExpression re, string name, Action<DefinitionRegistrationExpression> registration = null)
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

		public static DefinitionRegistrationExpression EndContainer(this DefinitionRegistrationExpression re)
		{
			if (re == null) return re;

			re.ContainerName = null;
			return re;
		}

		// editables

		public static DefinitionRegistrationExpression Title(this DefinitionRegistrationExpression re, string title = "Title", Action<WithEditableTitleAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new WithEditableTitleAttribute(), title, config);
		}

		public static DefinitionRegistrationExpression Name(this DefinitionRegistrationExpression re, string title = "Name", Action<WithEditableNameAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new WithEditableNameAttribute(), title, config);
		}

		public static DefinitionRegistrationExpression PublishedRange(this DefinitionRegistrationExpression re, string title = "Published between", Action<WithEditablePublishedRangeAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new WithEditablePublishedRangeAttribute(), title, config);
		}

		public static DefinitionRegistrationExpression DateRange(this DefinitionRegistrationExpression re, string nameStart, string nameEnd, string title = "Name", Action<WithEditableDateRangeAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new WithEditableDateRangeAttribute(title, 0, nameStart, nameEnd), title, config);
		}

		public static DefinitionRegistrationExpression CheckBox(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableCheckBoxAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddContainable(new EditableCheckBoxAttribute(title ?? name, re.NextSortOrder(null)), config);
		}

		public static DefinitionRegistrationExpression Children(this DefinitionRegistrationExpression re, string zoneName, string name = null, string title = null, Action<EditableChildrenAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddContainable(new EditableChildrenAttribute(title ?? zoneName, zoneName, re.NextSortOrder(null)), config);
		}

		public static DefinitionRegistrationExpression Date(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableDateAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableDateAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression Enum(this DefinitionRegistrationExpression re, string name, Type enumType, string title = null, Action<EditableEnumAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableEnumAttribute(enumType), name, title, config);
		}

		public static DefinitionRegistrationExpression FileUpload(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableFileUploadAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableFileUploadAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression FreeTextArea(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableFreeTextAreaAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableFreeTextAreaAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression Image(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableImageAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableImageAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression ImageSize(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableImageSizeAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableImageSizeAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression Item(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableItemAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableItemAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression LanguagesDropDown(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableLanguagesDropDownAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableLanguagesDropDownAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression Link(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableLinkAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableLinkAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression TextBox(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableTextBoxAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableTextBoxAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression ThemeSelection(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableThemeSelectionAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableThemeSelectionAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression Url(this DefinitionRegistrationExpression re, string name, string title = null, Action<EditableUrlAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableUrlAttribute(), name, title, config);
		}

		public static DefinitionRegistrationExpression UserControl(this DefinitionRegistrationExpression re, string name, string userControlPath, string title = null, Action<EditableUserControlAttribute> config = null)
		{
			if (re == null) return re;

			return re.AddEditable(new EditableUserControlAttribute(userControlPath, 0) { UserControlPath = userControlPath }, name, title, config);
		}
	}

}