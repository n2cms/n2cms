using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using System.Linq.Expressions;
using N2.Integrity;
using N2.Web.UI;

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



		/// <summary>
		/// Defines a tab panel that can be used to contain editor controls.
		/// </summary>
		public static ContainerBuilder<TModel, TabContainerAttribute> Tab<TModel>(this IContentRegistration<TModel> registration, string containerName, Action<IContentRegistration<TModel>> containerRegistration = null)
		{
			return registration.Register(new TabContainerAttribute(containerName, containerName, 0))
				.Do(containerRegistration);
		}

		/// <summary>
		/// Defines a tab panel that can be used to contain editor controls.
		/// </summary>
		public static ContainerBuilder<TModel, TabContainerAttribute> Tab<TModel>(this IContentRegistration<TModel> registration, string containerName, string tabText, Action<IContentRegistration<TModel>> containerRegistration = null)
		{
			return registration.Register(new TabContainerAttribute(containerName, tabText ?? containerName, 0))
				.Do(containerRegistration);
		}

		/// <summary>
		/// Defines a fieldset that can contain editors when editing an item.
		/// </summary>
		public static ContainerBuilder<TModel, FieldSetContainerAttribute> FieldSet<TModel>(this IContentRegistration<TModel> display, string containerName, string legend, Action<IContentRegistration<TModel>> containerRegistration = null)
		{
			return display.Register(new FieldSetContainerAttribute(containerName, legend ?? containerName, 0))
				.Do(containerRegistration);
		}

		/// <summary>
		/// Organizes editors in a field set that can be expanded to show all details.
		/// </summary>
		public static ContainerBuilder<TModel, ExpandableContainerAttribute> ExpandableContainer<TModel>(this IContentRegistration<TModel> display, string containerName, string legend, Action<IContentRegistration<TModel>> containerRegistration = null)
		{
			return display.Register(new ExpandableContainerAttribute(containerName, legend ?? containerName, 0))
				.Do(containerRegistration);
		}

		/// <summary>
		/// Places a container in the right-hand side of the editing UI.
		/// </summary>
		public static ContainerBuilder<TModel, SidebarContainerAttribute> Sidebar<TModel>(this IContentRegistration<TModel> display, string containerName, Action<IContentRegistration<TModel>> containerRegistration = null)
		{
			return display.Register(new SidebarContainerAttribute(containerName, 0) { HeadingText = containerName })
				.Do(containerRegistration);
		}

		/// <summary>
		/// Places a container in the right-hand side of the editing UI.
		/// </summary>
		public static ContainerBuilder<TModel, SidebarContainerAttribute> Sidebar<TModel>(this IContentRegistration<TModel> display, string containerName, string headingText, Action<IContentRegistration<TModel>> containerRegistration = null)
		{
			return display.Register(new SidebarContainerAttribute(containerName, 0) { HeadingText = headingText })
				.Do(containerRegistration);
		}

		/// <summary>
		/// Places contained controls in the site editor interface instead of the regular editor 
		/// interface. Any recursive containers in the selected page and it's ancestors are displayed.
		/// </summary>
		public static ContainerBuilder<TModel, RecursiveContainerAttribute> Recursive<TModel>(this IContentRegistration<TModel> display, string containerName, string headingFormat, Action<IContentRegistration<TModel>> containerRegistration = null)
		{
			return display.Register(new RecursiveContainerAttribute(containerName, 0) { HeadingFormat = headingFormat })
				.Do(containerRegistration);
		}

		public static ContainerBuilder<TModel, TContainer> Do<TModel, TContainer>(this ContainerBuilder<TModel, TContainer> container, Action<IContentRegistration<TModel>> containerRegistration = null)
			where TContainer : IEditableContainer
		{
			if (containerRegistration != null)
			{
				using (container.Begin())
				{
					containerRegistration(container.Registration);
				}
			}
			return container;
		}



		/// <summary>Class applicable attribute used to add a title editor.</summary>
		public static EditableBuilder<WithEditableTitleAttribute> Title<T>(this IContentRegistration<T> registration, string title = "Title")
		{
			return registration.RegisterEditable<WithEditableTitleAttribute>("Title", title);
		}

		/// <summary>Class applicable attribute used to add a name editor. The name represents the URL slug for a certain content item.</summary>
		public static EditableBuilder<WithEditableNameAttribute> Name<T>(this IContentRegistration<T> registration, string title = "Name")
		{
			return registration.RegisterEditable<WithEditableNameAttribute>("Name", title);
		}

		/// <summary>
		/// Class applicable editable attribute that adds text boxes for selecting 
		/// published date range.
		/// </summary>
		public static EditableBuilder<WithEditablePublishedRangeAttribute> PublishedRange<T>(this IContentRegistration<T> registration, string title = "Published between")
		{
			return registration.RegisterEditable<WithEditablePublishedRangeAttribute>("Published", title);
		}

		/// <summary>
		/// Decorates the content item with a date range editable that will update two date fields.
		/// </summary>
		public static EditableBuilder<WithEditableDateRangeAttribute> DateRange(this IPropertyRegistration<DateTime> registration, Expression<Func<DateTime>> endExpression, string title = "Dates")
		{
			string endExpressionText = System.Web.Mvc.ExpressionHelper.GetExpressionText(endExpression);
			return registration.Registration.RegisterEditable<WithEditableDateRangeAttribute>(registration.PropertyName, title)
				.Configure(a => a.NameEndRange = endExpressionText);
		}

		/// <summary>An editable checkbox attribute. Besides creating a checkbox it also uses the checkbox's text property to display text.</summary>
		public static EditableBuilder<EditableCheckBoxAttribute> CheckBox(this IPropertyRegistration<bool> registration, string checkBoxText = null)
		{
			return registration.Registration.RegisterEditable<EditableCheckBoxAttribute>(registration.PropertyName, "")
				.Configure(e => e.CheckBoxText = checkBoxText);
		}

		/// <summary>
		/// Defines a deletedChild item editor. Renders a drop down list where you can 
		/// select what item to add and edit forms of added items.
		/// </summary>
		public static EditableBuilder<EditableChildrenAttribute> Children<TContent>(this IPropertyRegistration<IEnumerable<TContent>> registration, string title = null) where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableChildrenAttribute>(registration.PropertyName, title ?? registration.PropertyName);
		}

		/// <summary>
		/// Defines an editable date/time picker control for a content item.
		/// </summary>
		public static EditableBuilder<EditableDateAttribute> Date(this IPropertyRegistration<DateTime> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableDateAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Adds a drop down for editing values in an enum attribute.
		/// </summary>
		public static EditableBuilder<EditableEnumAttribute> Enum<TEnum>(this IPropertyRegistration<TEnum> registration, string title = null) where TEnum : struct
		{
			if (!typeof(TEnum).IsEnum)
				throw new ArgumentException("Property " + registration.PropertyName + " of type " + typeof(TEnum) + " cannot be used with Enum registration. Only enum types are allowed");

			return registration.Registration.RegisterEditable<EditableEnumAttribute>(registration.PropertyName, title)
				.Configure(e => e.EnumType = typeof(TEnum));
		}

		/// <summary>
		/// An abstract base class that implements editable drop down functionality.
		/// Override and implement GetListItems to use.
		/// </summary>
		public static EditableBuilder<EditableDropDownAttribute> DropDown(this IPropertyRegistration<string> registration, params System.Web.UI.WebControls.ListItem[] listItems)
		{
			return registration.Registration.RegisterEditable<EditableDropDownAttribute>(new CustomDropDownAttribute() { Name = registration.PropertyName, Title = registration.PropertyName, ListItems = listItems });
		}

		/// <summary>
		/// An abstract base class that implements editable drop down functionality.
		/// Override and implement GetListItems to use.
		/// </summary>
		public static EditableBuilder<EditableDropDownAttribute> DropDown(this IPropertyRegistration<int> registration, params System.Web.UI.WebControls.ListItem[] listItems)
		{
			return registration.Registration.RegisterEditable<EditableDropDownAttribute>(new CustomDropDownAttribute() { Name = registration.PropertyName, Title = registration.PropertyName, ListItems = listItems });
		}

		/// <summary>
		/// An abstract base class that implements editable drop down functionality.
		/// Override and implement GetListItems to use.
		/// </summary>
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

		/// <summary>
		/// Allows selecting an item of a specific type from a drop down list.
		/// </summary>
		public static EditableBuilder<EditableItemSelectionAttribute> ItemSelection<TContent>(this IPropertyRegistration<TContent> registration, string title = null) where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableItemSelectionAttribute>(registration.PropertyName, title)
				.Configure(ee => ee.LinkedType = typeof(TContent));
		}

		/// <summary>
		/// Allows selecting zero or more items of a specific type from an exapandable check box list.
		/// </summary>
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

		/// <summary>
		/// Allows selecting zero or more items from an exapandable check box list.
		/// </summary>
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

		/// <summary>
		/// Allows to upload or select a file to use.
		/// </summary>
		public static EditableBuilder<EditableFileUploadAttribute> FileUpload(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableFileUploadAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Allows to upload or select a media file to use.
		/// </summary>
		public static EditableBuilder<EditableMediaUploadAttribute> MediaUpload(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableMediaUploadAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Defines an editable image this allows to select an image with the file picker.
		/// </summary>
		public static EditableBuilder<EditableFolderSelectionAttribute> FolderSelection(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableFolderSelectionAttribute>(registration.PropertyName, title);
		}

		/// <summary>Specifies the usage of a free text area editors to edit the property.</summary>
		public static EditableBuilder<EditableFreeTextAreaAttribute> FreeText(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableFreeTextAreaAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Defines an editable image this allows to select an image with the file picker.
		/// </summary>
		public static EditableBuilder<EditableImageAttribute> Image(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableImageAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Allows selecting between configured image sizes from a drop down list.
		/// </summary>
		public static EditableBuilder<EditableImageSizeAttribute> ImageSize(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableImageSizeAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Defines an editable item. The edited item is referenced by the 
		/// property decorated with this attribute. If the property is null a new
		/// item is created and added to the parent items child collection.
		/// </summary>
		public static EditableBuilder<EditableItemAttribute> Item<TContent>(this IPropertyRegistration<TContent> registration, string title = null) where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableItemAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// An editable drop down with cultures/languages.
		/// </summary>
		public static EditableBuilder<EditableLanguagesDropDownAttribute> Languages(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableLanguagesDropDownAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Defines an editable link to another item on this site. The item is 
		/// selected through a popup window displaying the item tree.
		/// </summary>
		public static EditableBuilder<EditableLinkAttribute> Link<TContent>(this IPropertyRegistration<TContent> registration, string title = null) where TContent : ContentItem
		{
			return registration.Registration.RegisterEditable<EditableLinkAttribute>(registration.PropertyName, title)
				.Configure(ee => ee.SelectableTypes = new [] { typeof(TContent) });
		}

		/// <summary>
		/// Specifices usage of an <see cref="System.Web.UI.WebControls.TextBox"/> web control as editor.</summary>
		/// <example>
		public static EditableBuilder<EditableTextAttribute> Text(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableTextAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Specifies a text box which renders as a html meta tag element when rendered on a web page.
		/// </summary>
		public static EditableBuilder<EditableMetaTagAttribute> Meta(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableMetaTagAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Allows the selection of themes.
		/// </summary>
		public static EditableBuilder<EditableThemeSelectionAttribute> ThemeSelection(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableThemeSelectionAttribute>(registration.PropertyName, title);
		}

		/// <summary>Attribute used to mark properties as editable. This attribute is predefined to use the <see cref="N2.Web.UI.WebControls.UrlSelector"/> web control as editor/url selector.</summary>
		public static EditableBuilder<EditableUrlAttribute> Url(this IPropertyRegistration<string> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableUrlAttribute>(registration.PropertyName, title);
		}

		/// <summary>Attribute used to mark properties as editable. This is used to associate the control used for the editing with the property/detail on the content item whose value we are editing.</summary>
		public static EditableBuilder<EditableUserControlAttribute> UserControl(this IPropertyRegistration registration, string userControlPath, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableUserControlAttribute>(registration.PropertyName, title)
				.Configure(e => e.UserControlPath = userControlPath);
		}

		/// <summary>
		/// Extracts a summary text from another detail and stores
		/// </summary>
		public static EditableBuilder<EditableSummaryAttribute> Summary(this IPropertyRegistration<string> registration, string title = null, string source = null)
		{
			return registration.Registration.RegisterEditable<EditableSummaryAttribute>(registration.PropertyName, title)
				.Configure(e => e.Source = source);
		}

		/// <summary>
		/// Allows editing tags on an item.
		/// </summary>
		public static EditableBuilder<EditableTagsAttribute> Tags(this IPropertyRegistration<IEnumerable<string>> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableTagsAttribute>(registration.PropertyName, title);
		}

		/// <summary>
		/// Adds editing of a number.
		/// </summary>
		public static EditableBuilder<EditableNumberAttribute> Number(this IPropertyRegistration<int> registration, string title = null)
		{
			return registration.Registration.RegisterEditable<EditableNumberAttribute>(registration.PropertyName, title);
		}



		/// <summary>
		/// A class decoration used to restrict which items may be placed under 
		/// which. When this attribute intersects with 
		/// <see cref="AllowedChildrenAttribute"/>, the union of these two are 
		/// considered to be allowed.</summary>
		public static Builder<RestrictParentsAttribute> RestrictParents<TModel>(this IContentRegistration<TModel> registration, AllowedTypes allowedTypes)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(allowedTypes));
		}

		/// <summary>
		/// A class decoration used to restrict which items may be placed under 
		/// which. When this attribute intersects with 
		/// <see cref="AllowedChildrenAttribute"/>, the union of these two are 
		/// considered to be allowed.</summary>
		public static Builder<RestrictParentsAttribute> RestrictParents<TModel>(this IContentRegistration<TModel> registration, params Type[] allowedParentTypes)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(allowedParentTypes));
		}

		/// <summary>
		/// A class decoration used to restrict which items may be placed under 
		/// which. When this attribute intersects with 
		/// <see cref="AllowedChildrenAttribute"/>, the union of these two are 
		/// considered to be allowed.</summary>
		public static Builder<RestrictParentsAttribute> RestrictParents<TModel>(this IContentRegistration<TModel> registration, params string[] allowedParentTemplateKeys)
		{
			return registration.RegisterRefiner<RestrictParentsAttribute>(new RestrictParentsAttribute(AllowedTypes.All) { TemplateKeys = allowedParentTemplateKeys });
		}

		/// <summary>
		/// This attribute replace the children allowed with the types 
		/// </summary>
		public static Builder<RestrictChildrenAttribute> RestrictChildren<TModel>(this IContentRegistration<TModel> registration, AllowedTypes allowedTypes)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(allowedTypes));
		}

		/// <summary>
		/// This attribute replace the children allowed with the types 
		/// </summary>
		public static Builder<RestrictChildrenAttribute> RestrictChildren<TModel>(this IContentRegistration<TModel> registration, params Type[] allowedChildTypes)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(allowedChildTypes));
		}

		/// <summary>
		/// This attribute replace the children allowed with the types 
		/// </summary>
		public static Builder<RestrictChildrenAttribute> RestrictChildren<TModel>(this IContentRegistration<TModel> registration, params string[] allowedChildTemplateKeys)
		{
			return registration.RegisterRefiner<RestrictChildrenAttribute>(new RestrictChildrenAttribute(AllowedTypes.All) { TemplateNames = allowedChildTemplateKeys });
		}

		/// <summary>
		/// Setting Enabled to false on this attribute prevents it from being added to it's parent's child collection.
		/// </summary>
		public static Builder<IDefinitionRefiner> Sort<TModel>(this IContentRegistration<TModel> registration, SortBy sortingOrder, string expression = null)
		{
			return registration.RegisterRefiner<IDefinitionRefiner>(new AppendAttributeRefiner(new SiblingInsertionAttribute(sortingOrder) { SortExpression = expression }));
		}

		/// <summary>
		/// Controls the order of children added to items decorated with this attribute.
		/// </summary>
		public static Builder<IDefinitionRefiner> SortChildren<TModel>(this IContentRegistration<TModel> registration, SortBy sortingOrder, string expression = null)
		{
			return registration.RegisterRefiner<IDefinitionRefiner>(new AppendAttributeRefiner(new SortChildrenAttribute(sortingOrder) { SortExpression = expression }));
		}

		/// <summary>Specifies the icon to display when selecting new items to create.</summary>
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
