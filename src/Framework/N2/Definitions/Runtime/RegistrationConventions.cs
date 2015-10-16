using System;
using System.Collections.Generic;
using N2.Details;

namespace N2.Definitions.Runtime
{
	public class RegistrationConventions
	{
		public RegistrationConventions()
		{
			Containers = id => new IEditableContainer[0];
			EditableContainer = pd => null;
			Editable = DefaultEditableFactory;
			Displayable = pd => null;
			Finalize = pd => { };
		}

		public Func<ItemDefinition, IEnumerable<IEditableContainer>> Containers { get; set; }
		public Func<PropertyDefinition, string> EditableContainer { get; set; }
		public Func<PropertyDefinition, IEditable> Editable { get; set; }
		public Func<PropertyDefinition, IDisplayable> Displayable { get; set; }
		public Action<ItemDefinition> Finalize { get; set; }

		public static IEditable DefaultEditableFactory(PropertyDefinition pd)
		{
			if (pd.Editable != null)
				return null;


			if (pd.Info != null && pd.Info.DeclaringType == typeof(ContentItem))
			{
				if (pd.Name == "Title")
					return new WithEditableTitleAttribute();
				if (pd.Name == "Name")
					return new WithEditableNameAttribute();

				return null;
			}

			if (pd.PropertyType == typeof(string))
			{
				if (pd.Name.Contains("Image"))
					return new EditableImageUploadAttribute();
				if (pd.Name.Contains("Url"))
					return new EditableUrlAttribute();
				if (pd.Name.Contains("Text") || pd.Name.Contains("Body") || pd.Name.Contains("Main"))
					return new EditableFreeTextAreaAttribute();
				if (pd.Name.Contains("Theme"))
					return new EditableThemeSelectionAttribute();
				if (pd.Name.Contains("Language"))
					return new EditableLanguagesDropDownAttribute();
				if (pd.Name.Contains("Media"))
					return new EditableMediaUploadAttribute();
				if (pd.Name.Contains("Meta"))
					return new EditableMetaTagAttribute();
				if (pd.Name.Contains("Folder"))
					return new EditableFolderSelectionAttribute();
				if (pd.Name.Contains("File"))
					return new EditableFileUploadAttribute();

				return new EditableTextAttribute();
			}

			if (pd.PropertyType == typeof(DateTime) || pd.PropertyType == typeof(DateTime?))
				return new EditableDateAttribute();

			if (pd.PropertyType == typeof(int))
				return new EditableNumberAttribute();

			if (pd.PropertyType.IsEnum)
				return new EditableEnumAttribute(pd.PropertyType);

			if (pd.PropertyType == typeof(ContentItem))
				return new EditableLinkAttribute();

			if (typeof(ContentItem).IsAssignableFrom(pd.PropertyType))
				return new EditableItemSelectionAttribute(pd.PropertyType);

			if (pd.PropertyType.IsGenericType)
			{
				if (typeof(IEnumerable<>).IsAssignableFrom(pd.PropertyType.GetGenericTypeDefinition()))
				{
					var genericArgument = pd.PropertyType.GetGenericArguments()[0];
					if (typeof(ContentItem).IsAssignableFrom(genericArgument))
					{
						if (pd.Name.Contains("Children"))
							return new EditableChildrenAttribute() { ZoneName = pd.Name };

						return new EditableMultipleItemSelectionAttribute(genericArgument);
					}
					if (genericArgument == typeof(string))
					{
						if (pd.Name.Contains("Tag"))
							return new EditableTagsAttribute();
					}
				}
				return null;
			}

			return null;
		}
	}
}
