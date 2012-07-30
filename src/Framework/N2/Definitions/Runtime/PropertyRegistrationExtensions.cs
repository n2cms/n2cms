using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using System.Linq.Expressions;
using N2.Integrity;

namespace N2.Definitions.Runtime
{
	public static class PropertyRegistrationExtensions
	{
		/// <summary>Begins registration of a property on the content type.</summary>
		/// <typeparam name="TProperty">The property type of the property.</typeparam>
		/// <param name="expression">An expression defining the property, e.g. item =&gt; item.Text</param>
		/// <returns>A property registration object.</returns>
		public static PropertyRegistration<TModel, TProperty> On<TModel, TProperty>(this IContentRegistration<TModel> registration, Expression<Func<TModel, TProperty>> expression)
		{
			string expressionText = System.Web.Mvc.ExpressionHelper.GetExpressionText(expression);
			return new PropertyRegistration<TModel, TProperty>(registration, expressionText);
		}

		public static EditableBuilder<WithEditableTitleAttribute> Title<T>(this IContentRegistration<T> registration, string title = "Title")
		{
			return registration.RegisterEditable<WithEditableTitleAttribute>("Title", title);
		}

		public static EditableBuilder<WithEditableNameAttribute> Name<T>(this IContentRegistration<T> registration, string title = "Name")
		{
			return registration.RegisterEditable<WithEditableNameAttribute>("Name", title);
		}

		public static EditableBuilder<WithEditablePublishedRangeAttribute> PublishedRange<T>(this IContentRegistration<T> registration, string title = "Published between")
		{
			return registration.RegisterEditable<WithEditablePublishedRangeAttribute>("Published", title);
		}

		public static EditableBuilder<WithEditableDateRangeAttribute> DateRange(this IPropertyRegistration<DateTime> registration, Expression<Func<DateTime>> endExpression, string title = "Dates")
		{
			string endExpressionText = System.Web.Mvc.ExpressionHelper.GetExpressionText(endExpression);
			return registration.Registration.RegisterEditable<WithEditableDateRangeAttribute>(registration.PropertyName, title)
				.Configure(a => a.NameEndRange = endExpressionText);
		}

		public static EditableBuilder<EditableCheckBoxAttribute> CheckBox(this IPropertyRegistration<bool> registration, string checkBoxText = null)
		{
			return registration.Registration.RegisterEditable<EditableCheckBoxAttribute>(registration.PropertyName, "")
				.Configure(e => e.CheckBoxText = checkBoxText);
		}

		public static EditableBuilder<EditableChildrenAttribute> Children<TContent>(this IPropertyRegistration<IEnumerable<TContent>> registration, string title = null) where TContent: ContentItem
		{
			return registration.Registration.RegisterEditable<EditableChildrenAttribute>(registration.PropertyName, title ?? registration.PropertyName);
		}

		public static EditableBuilder<EditableDateAttribute> Date(this IPropertyRegistration<DateTime> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableDateAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableEnumAttribute> Enum<TEnum>(this IPropertyRegistration<TEnum> registration, string title = null) where TEnum : struct
		{
			if (!typeof(TEnum).IsEnum)
				throw new ArgumentException("Property " + registration.PropertyName + " of type " + typeof(TEnum) + " cannot be used with Enum registration. Only enum types are allowed");

			return registration.Registration.RegisterEditable<EditableEnumAttribute>(registration.PropertyName, title)
				.Configure(e => e.EnumType = typeof(TEnum));
		}

		public static EditableBuilder<EditableDropDownAttribute> DropDown(this IPropertyRegistration<string> registration, params System.Web.UI.WebControls.ListItem[] listItems)
		{
			return registration.Registration.RegisterEditable<EditableDropDownAttribute>(new CustomDropDownAttribute() { Name = registration.PropertyName, Title = registration.PropertyName, ListItems = listItems });
		}

		public static EditableBuilder<EditableDropDownAttribute> DropDown(this IPropertyRegistration<int> registration, params System.Web.UI.WebControls.ListItem[] listItems)
		{
			return registration.Registration.RegisterEditable<EditableDropDownAttribute>(new CustomDropDownAttribute() { Name = registration.PropertyName, Title = registration.PropertyName, ListItems = listItems });
		}

		public static EditableBuilder<EditableDropDownAttribute> DropDown(this IPropertyRegistration<string> registration, params string[] listItems)
		{
			return registration.DropDown(listItems.Select(li => new System.Web.UI.WebControls.ListItem(li)).ToArray());
		}

		class CustomDropDownAttribute : EditableDropDownAttribute
		{
			public System.Web.UI.WebControls.ListItem[] ListItems { get; set; }
			protected override System.Web.UI.WebControls.ListItem[] GetListItems()
			{
				return ListItems;
			}
		}

		public static EditableBuilder<EditableItemSelectionAttribute> ItemSelection<TContent>(this IPropertyRegistration<TContent> registration, string title = null) where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableItemSelectionAttribute>(registration.PropertyName, title)
				.Configure(ee => ee.LinkedType = typeof(TContent));
		}

		public static EditableBuilder<EditableMultipleItemSelectionAttribute> MultipleItemSelection<TContent>(this IPropertyRegistration<IEnumerable<TContent>> registration, Type excludedType = null) where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableMultipleItemSelectionAttribute>(new EditableMultipleItemSelectionAttribute
			{
				LinkedType = typeof(TContent),
				ExcludedType = excludedType ?? typeof(ISystemNode),
				Title = registration.PropertyName,
				Name = registration.PropertyName
			});
		}

		public static EditableBuilder<EditableMultipleItemSelectionAttribute> MultipleItemSelection<TContent>(this IPropertyRegistration<IEnumerable<TContent>> registration, Func<IEnumerable<ContentItem>> getContentItems) where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableMultipleItemSelectionAttribute>(new CustomMultipleItemSelection
			{
				Title = registration.PropertyName,
				Name = registration.PropertyName,
				CustomItemsGetter = () => getContentItems().Select(ci => new System.Web.UI.WebControls.ListItem(ci.Title, ci.ID.ToString()))
			});
		}

		class CustomMultipleItemSelection : EditableMultipleItemSelectionAttribute
		{
			protected override System.Web.UI.WebControls.ListItem[] GetListItems()
			{
				return CustomItemsGetter().ToArray();
			}

			public Func<IEnumerable<System.Web.UI.WebControls.ListItem>> CustomItemsGetter { get; set; }
		}

		public static EditableBuilder<EditableFileUploadAttribute> FileUpload(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableFileUploadAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableMediaUploadAttribute> MediaUpload(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableMediaUploadAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableFolderSelectionAttribute> FolderSelection(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableFolderSelectionAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableFreeTextAreaAttribute> FreeText(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableFreeTextAreaAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableImageAttribute> Image(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableImageAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableImageSizeAttribute> ImageSize(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableImageSizeAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableItemAttribute> Item<TContent>(this IPropertyRegistration<TContent> registration, string title = null)  where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableItemAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableLanguagesDropDownAttribute> Languages(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableLanguagesDropDownAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableLinkAttribute> Link<TContent>(this IPropertyRegistration<TContent> registration, string title = null) where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableLinkAttribute>(registration.PropertyName, title)
				.Configure(ee => ee.SelectableTypes = new [] { typeof(TContent) });
		}

		public static EditableBuilder<EditableTextAttribute> Text(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableTextAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableMetaTagAttribute> Meta(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableMetaTagAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableThemeSelectionAttribute> ThemeSelection(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableThemeSelectionAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableUrlAttribute> Url(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableUrlAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableUserControlAttribute> UserControl(this IPropertyRegistration registration, string userControlPath, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableUserControlAttribute>(registration.PropertyName, title)
				.Configure(e => e.UserControlPath = userControlPath);
		}

		public static EditableBuilder<EditableSummaryAttribute> Summary(this IPropertyRegistration<string> registration, string title = null, string source = null)
		{
			return registration.Registration.RegisterEditable<EditableSummaryAttribute>(registration.PropertyName, title)
				.Configure(e => e.Source = source);
		}

		public static EditableBuilder<EditableTagsAttribute> Tags(this IPropertyRegistration<IEnumerable<string>> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableTagsAttribute>(registration.PropertyName, title);
		}

		public static EditableBuilder<EditableNumberAttribute> Number(this IPropertyRegistration<int> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableNumberAttribute>(registration.PropertyName, title);
		}



		public static Builder<RestrictParentsAttribute> RestrictParents<TModel>(this IContentRegistration<TModel> registration, AllowedTypes allowedTypes)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(allowedTypes));
		}

		public static Builder<RestrictParentsAttribute> RestrictParents<TModel>(this IContentRegistration<TModel> registration, params Type[] allowedParentTypes)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(allowedParentTypes));
		}

		public static Builder<RestrictParentsAttribute> RestrictParents<TModel>(this IContentRegistration<TModel> registration, params string[] allowedParentTemplateKeys)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(AllowedTypes.All) { TemplateKeys = allowedParentTemplateKeys });
		}

		public static Builder<RestrictChildrenAttribute> RestrictChildren<TModel>(this IContentRegistration<TModel> registration, AllowedTypes allowedTypes)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(allowedTypes));
		}

		public static Builder<RestrictChildrenAttribute> RestrictChildren<TModel>(this IContentRegistration<TModel> registration, params Type[] allowedChildTypes)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(allowedChildTypes));
		}

		public static Builder<RestrictChildrenAttribute> RestrictChildren<TModel>(this IContentRegistration<TModel> registration, params string[] allowedChildTemplateKeys)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(AllowedTypes.All) { TemplateNames = allowedChildTemplateKeys });
		}

		public static Builder<IDefinitionRefiner> Sort<TModel>(this IContentRegistration<TModel> registration, SortBy sortingOrder, string expression = null)
		{
			return registration.RegisterRefiner<IDefinitionRefiner>(new AppendAttributeRefiner(new SiblingInsertionAttribute(sortingOrder) { SortExpression = expression }));
		}

		public static Builder<IDefinitionRefiner> SortChildren<TModel>(this IContentRegistration<TModel> registration, SortBy sortingOrder, string expression = null)
		{
			return registration.RegisterRefiner<IDefinitionRefiner>(new AppendAttributeRefiner(new SortChildrenAttribute(sortingOrder) { SortExpression = expression }));
		}

		public static IContentRegistration<TModel> Icon<TModel>(this IContentRegistration<TModel> registration, string iconUrl)
		{
			registration.Definition.IconUrl = N2.Web.Url.ResolveTokens(iconUrl);
			return registration;
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
