using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Definitions;
using N2.Details;

namespace N2.Web.Mvc.Html
{
	public static class Extensions
	{
		public static IHtmlString ToHtmlString(this object instance)
		{
			return new HtmlString(instance.ToString());
		}
	}

	public static class RegisterExtensions
	{
		public static RegistrationExpression RegisterEditables<T>(this ContentContext<T> content, Action<RegistrationExpression> registration = null) where T: class
		{
			var re = content.Html.ViewContext.HttpContext.Items["RegistrationExpression"] as RegistrationExpression;
			if(re != null)
				registration(re);
			return re;
		}

		// containables

		public static RegistrationExpression Tab(this RegistrationExpression re, string containerName, string tabName, Action<RegistrationExpression> registration = null, int? sortOrder = null)
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

		public static RegistrationExpression FieldSet(this RegistrationExpression re, string containerName, string legend, Action<RegistrationExpression> registration = null, int? sortOrder = null)
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

		public static RegistrationExpression Container(this RegistrationExpression re, string name, Action<RegistrationExpression> registration = null)
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

		public static RegistrationExpression EndContainer(this RegistrationExpression re)
		{
			if (re == null) return re;

			re.ContainerName = null;
			return re;
		}

		// editables
		
		public static RegistrationExpression Title(this RegistrationExpression re, string title = "Title", Action<WithEditableTitleAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new WithEditableTitleAttribute(), title, config);
		}

		public static RegistrationExpression Name(this RegistrationExpression re, string title = "Name", Action<WithEditableNameAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new WithEditableNameAttribute(), title, config);
		}

		public static RegistrationExpression PublishedRange(this RegistrationExpression re, string title = "Published between", Action<WithEditablePublishedRangeAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new WithEditablePublishedRangeAttribute(), title, config);
		}

		public static RegistrationExpression DateRange(this RegistrationExpression re, string nameStart, string nameEnd, string title = "Name", Action<WithEditableDateRangeAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new WithEditableDateRangeAttribute(title, 0, nameStart, nameEnd), title, config);
		}

		public static RegistrationExpression CheckBox(this RegistrationExpression re, string name, string title = null, Action<EditableCheckBoxAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableCheckBoxAttribute(title ?? name, re.NextSortOrder(null)), config);
		}

		public static RegistrationExpression Children(this RegistrationExpression re, string zoneName, string name = null, string title = null, Action<EditableChildrenAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableChildrenAttribute(title ?? zoneName, zoneName, re.NextSortOrder(null)), config);
		}

		public static RegistrationExpression Date(this RegistrationExpression re, string name, string title = null, Action<EditableDateAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableDateAttribute(), name, title, config);
		}

		public static RegistrationExpression Enum(this RegistrationExpression re, string name, Type enumType, string title = null, Action<EditableEnumAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableEnumAttribute(enumType), name, title, config);
		}

		public static RegistrationExpression FileUpload(this RegistrationExpression re, string name, string title = null, Action<EditableFileUploadAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableFileUploadAttribute(), name, title, config);
		}

		public static RegistrationExpression FreeTextArea(this RegistrationExpression re, string name, string title = null, Action<EditableFreeTextAreaAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableFreeTextAreaAttribute(), name, title, config);
		}

		public static RegistrationExpression Image(this RegistrationExpression re, string name, string title = null, Action<EditableImageAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableImageAttribute(), name, title, config);
		}

		public static RegistrationExpression ImageSize(this RegistrationExpression re, string name, string title = null, Action<EditableImageSizeAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableImageSizeAttribute(), name, title, config);
		}

		public static RegistrationExpression Item(this RegistrationExpression re, string name, string title = null, Action<EditableItemAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableItemAttribute(), name, title, config);
		}

		public static RegistrationExpression LanguagesDropDown(this RegistrationExpression re, string name, string title = null, Action<EditableLanguagesDropDownAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableLanguagesDropDownAttribute(), name, title, config);
		}

		public static RegistrationExpression Link(this RegistrationExpression re, string name, string title = null, Action<EditableLinkAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableLinkAttribute(), name, title, config);
		}

		public static RegistrationExpression TextBox(this RegistrationExpression re, string name, string title = null, Action<EditableTextBoxAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableTextBoxAttribute(), name, title, config);
		}

		public static RegistrationExpression ThemeSelection(this RegistrationExpression re, string name, string title = null, Action<EditableThemeSelectionAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableThemeSelectionAttribute(), name, title, config);
		}

		public static RegistrationExpression Url(this RegistrationExpression re, string name, string title = null, Action<EditableUrlAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableUrlAttribute(), name, title, config);
		}

		public static RegistrationExpression UserControl(this RegistrationExpression re, string name, string userControlPath, string title = null, Action<EditableUserControlAttribute> config = null)
		{
			if (re == null) return re;

			return re.Add(new EditableUserControlAttribute(userControlPath, 0) { UserControlPath = userControlPath }, name, title, config);
		}
	}

	public class RegistrationExpression
	{
		public RegistrationExpression()
		{
			Editables = new List<IEditable>();
			Containables = new List<IContainable>();
		}

		public RegistrationExpression Add<T>(T editable, Action<T> config) where T : IEditable
		{
			Editables.Add(editable);
			editable.ContainerName = ContainerName;
			if (config != null) config(editable);

			return this;
		}

		public RegistrationExpression Add<T>(T editable, string title, Action<T> config) where T : IEditable
		{
			Add(editable, config);
			editable.Title = title;
			editable.SortOrder = NextSortOrder(null);

			return this;
		}

		public RegistrationExpression Add<T>(T editable, string name, string title, Action<T> config) where T : IEditable
		{
			Add(editable, title ?? name, config);
			editable.Name = name;

			return this;
		}

		public RegistrationExpression AddContainable(IContainable containable)
		{
			Containables.Add(containable);
			containable.ContainerName = ContainerName;

			return this;
		}

		public int NextSortOrder(int? proposedSortOrder)
		{
			CurrentSortOrder = proposedSortOrder ?? ++CurrentSortOrder;
			return CurrentSortOrder;
		}

		public string ContainerName { get; set; }
		public int CurrentSortOrder { get; set; }
		public IList<IEditable> Editables { get; private set; }
		public IList<IContainable> Containables { get; private set; }
	}
}