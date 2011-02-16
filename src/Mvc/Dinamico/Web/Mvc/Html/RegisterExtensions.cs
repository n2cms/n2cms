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
				re.GlobalSortOffset = 0;
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
				re.GlobalSortOffset = 0;
				registration(re);
			}
			return re;
		}

		public static DefinitionRegistrationExpression PrependDefinition<T>(this ContentHelper<T> content, Action<DefinitionRegistrationExpression> registration = null) where T : class
		{
			var re = GetRegistrationExpression<T>(content.Html);
			if (re != null)
			{
				re.GlobalSortOffset = -1000;
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

			re.Add(new N2.Web.UI.TabContainerAttribute(containerName, tabName, re.NextSortOrder(sortOrder)));

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
			re.Add(new N2.Web.UI.FieldSetContainerAttribute(containerName, legend, re.NextSortOrder(sortOrder)));

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

		public static DefinitionRegistrationExpression Title(this DefinitionRegistrationExpression re, string title = "Title")
		{
			if (re == null) return re;

			return re.Add(new WithEditableTitleAttribute(), title);
		}

		public static DefinitionRegistrationExpression Name(this DefinitionRegistrationExpression re, string title = "Name")
		{
			if (re == null) return re;

			return re.Add(new WithEditableNameAttribute(), title);
		}

		public static DefinitionRegistrationExpression PublishedRange(this DefinitionRegistrationExpression re, string title = "Published between")
		{
			if (re == null) return re;

			return re.Add(new WithEditablePublishedRangeAttribute(), title);
		}

		public static DefinitionRegistrationExpression DateRange(this DefinitionRegistrationExpression re, string nameStart, string nameEnd, string title = "Name")
		{
			if (re == null) return re;

			return re.Add(new WithEditableDateRangeAttribute(title, 0, nameStart, nameEnd), title);
		}

		public static DefinitionRegistrationExpression CheckBox(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableCheckBoxAttribute(title ?? name, re.NextSortOrder(null)));
		}

		public static DefinitionRegistrationExpression Children(this DefinitionRegistrationExpression re, string zoneName, string name = null, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableChildrenAttribute(title ?? zoneName, zoneName, re.NextSortOrder(null)));
		}

		public static DefinitionRegistrationExpression Date(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableDateAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression Enum(this DefinitionRegistrationExpression re, string name, Type enumType, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableEnumAttribute(enumType), name, title);
		}

		public static DefinitionRegistrationExpression FileUpload(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableFileUploadAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression FreeTextArea(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableFreeTextAreaAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression Image(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableImageAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression ImageSize(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableImageSizeAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression Item(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableItemAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression LanguagesDropDown(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableLanguagesDropDownAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression Link(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableLinkAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression TextBox(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableTextBoxAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression ThemeSelection(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableThemeSelectionAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression Url(this DefinitionRegistrationExpression re, string name, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableUrlAttribute(), name, title);
		}

		public static DefinitionRegistrationExpression UserControl(this DefinitionRegistrationExpression re, string name, string userControlPath, string title = null)
		{
			if (re == null) return re;

			return re.Add(new EditableUserControlAttribute(userControlPath, 0) { UserControlPath = userControlPath }, name, title);
		}
	}

}