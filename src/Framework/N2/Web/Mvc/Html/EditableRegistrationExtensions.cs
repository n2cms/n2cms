using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions.Runtime;
using N2.Details;
using N2.Integrity;
using N2.Definitions;
using System.Web.UI.WebControls;

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

		public static EditableBuilder<EditableMultipleItemSelectionAttribute> MultipleItemSelection(this IContentRegistration registration, string name, Type linkedType, Type excludedType = null)
		{
			return registration.RegisterEditable<EditableMultipleItemSelectionAttribute>(new EditableMultipleItemSelectionAttribute
			{
				LinkedType = linkedType,
				ExcludedType = excludedType ?? typeof(ISystemNode),
				Title = name,
				Name = name
			});
		}

		public static EditableBuilder<EditableMultipleItemSelectionAttribute> MultipleItemSelection(this IContentRegistration registration, string name, Func<IEnumerable<ContentItem>> getContentItems)
		{
			return registration.RegisterEditable<EditableMultipleItemSelectionAttribute>(new CustomMultipleItemSelection
			{
				Title = name,
				Name = name,
				CustomItemsGetter = () => getContentItems().Select(ci => new ListItem(ci.Title, ci.ID.ToString()))
			});
		}

		class CustomMultipleItemSelection : EditableMultipleItemSelectionAttribute
		{
			protected override ListItem[] GetListItems()
			{
				return CustomItemsGetter().ToArray();
			}

			public Func<IEnumerable<ListItem>> CustomItemsGetter { get; set; }
		}

		public static EditableBuilder<EditableFileUploadAttribute> FileUpload(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableFileUploadAttribute>(name, title);
		}

		public static EditableBuilder<EditableMediaUploadAttribute> MediaUpload(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableMediaUploadAttribute>(name, title);
		}

		public static EditableBuilder<EditableFolderSelectionAttribute> FolderSelection(this IContentRegistration registration, string name, string title = null)
		{
			return registration.RegisterEditable<EditableFolderSelectionAttribute>(name, title);
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

		public static EditableBuilder<EditableSummaryAttribute> Summary(this IContentRegistration registration, string name, string title = null, string source = null)
		{
			return registration.RegisterEditable<EditableSummaryAttribute>(name, title)
				.Configure(e => e.Source = source);
		}

		public static Builder<RestrictParentsAttribute> RestrictParents(this IContentRegistration registration, AllowedTypes allowedTypes)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(allowedTypes));
		}

		public static Builder<RestrictParentsAttribute> RestrictParents(this IContentRegistration registration, params Type[] allowedParentTypes)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(allowedParentTypes));
		}

		public static Builder<RestrictParentsAttribute> RestrictParents(this IContentRegistration registration, params string[] allowedParentTemplateKeys)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(AllowedTypes.All) { TemplateKeys = allowedParentTemplateKeys });
		}

		public static Builder<RestrictChildrenAttribute> RestrictChildren(this IContentRegistration registration, AllowedTypes allowedTypes)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(allowedTypes));
		}

		public static Builder<RestrictChildrenAttribute> RestrictChildren(this IContentRegistration registration, params Type[] allowedChildTypes)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(allowedChildTypes));
		}

		public static Builder<RestrictChildrenAttribute> RestrictChildren(this IContentRegistration registration, params string[] allowedChildTemplateKeys)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(AllowedTypes.All) { TemplateNames = allowedChildTemplateKeys });
		}

		public static Builder<IDefinitionRefiner> Sort(this IContentRegistration registration, SortBy sortingOrder, string expression = null)
		{
			return registration.RegisterRefiner<IDefinitionRefiner>(new AppendAttributeRefiner(new SiblingInsertionAttribute(sortingOrder) { SortExpression = expression }));
		}

		public static Builder<IDefinitionRefiner> SortChildren(this IContentRegistration registration, SortBy sortingOrder, string expression = null)
		{
			return registration.RegisterRefiner<IDefinitionRefiner>(new AppendAttributeRefiner(new SortChildrenAttribute(sortingOrder) { SortExpression = expression }));
		}

		public static EditableBuilder<EditableTagsAttribute> Tags(this IContentRegistration registration, string name)
		{
			return registration.RegisterEditable<EditableTagsAttribute>(name, name);
		}

		class AppendAttributeRefiner : IDefinitionRefiner
		{
			private object attribute;

			public AppendAttributeRefiner(object attribute)
			{
				this.attribute = attribute;
			}

			public int RefinementOrder
			{
				get { return 0; }
			}

			public void Refine(ItemDefinition currentDefinition, System.Collections.Generic.IList<ItemDefinition> allDefinitions)
			{
				currentDefinition.Attributes.Add(attribute);
			}

			public int CompareTo(ISortableRefiner other)
			{
				return RefinementOrder - other.RefinementOrder;
			}
		}
	}

}