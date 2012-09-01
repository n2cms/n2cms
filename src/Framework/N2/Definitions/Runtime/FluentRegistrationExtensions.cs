using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Integrity;
using N2.Details;
using N2.Web.UI;
using System.Linq.Expressions;
using N2.Web;

namespace N2.Definitions.Runtime
{
	public static class FluentRegistrationExtensions
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

		// definition

		public static PageDefinitionAttribute Page<TModel>(this IContentRegistration<TModel> registration, string templateUrl = null, string title = null, string description = null)
		{
			var pda = new PageDefinitionAttribute(title ?? registration.Definition.ItemType.Name) { Description = description, TemplateUrl = templateUrl ?? CalculateUrl(registration.Definition.ItemType, ".aspx") };
			registration.RegisterRefiner(new DelayRefinement(pda));
			return pda;
		}

		public static PartDefinitionAttribute Part<TModel>(this IContentRegistration<TModel> registration, string templateUrl = null, string title = null, string description = null)
		{
			var pda = new PartDefinitionAttribute(title ?? registration.Definition.ItemType.Name) { Description = description, TemplateUrl = templateUrl ?? CalculateUrl(registration.Definition.ItemType, ".ascx") };
			registration.RegisterRefiner(new DelayRefinement(pda));
			return pda;
		}

		private static string CalculateUrl(Type type, string extension)
		{
			string typeName = type.FullName;
			string assemblyName = type.Assembly.GetName().Name;
			if (type.FullName.StartsWith(assemblyName))
				typeName = type.FullName.Substring(assemblyName.Length + 1);

			return "~/" + typeName.Replace('.', '/') + extension;
		}

		class DelayRefinement : ISortableRefiner
		{
			private ISimpleDefinitionRefiner inner;

			public DelayRefinement(ISimpleDefinitionRefiner inner)
			{
				this.inner = inner;
			}
			public int RefinementOrder
			{
				get { return int.MaxValue; }
			}

			public void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
			{
				inner.Refine(currentDefinition);
				if (inner is IPathFinder)
					PathDictionary.PrependFinder(currentDefinition.ItemType, inner as IPathFinder);
			}

			public int CompareTo(ISortableRefiner other)
			{
				return RefinementOrder - other.RefinementOrder;
			}
		}


		// containers

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

		// editables

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

		// restrictions

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

		/// <summary>Registers editors based on the passed conventions, or the defualt conventions</summary>
		/// <param name="registration"></param>
		/// <param name="interceptEditable">An intereption point for editables before they are registered. Another editable or null can be returned.</param>
		/// <param name="configureConventions">Configures conventions before they are used.</param>
		public static void UsingConventions<TModel>(this IContentRegistration<TModel> registration, Func<PropertyDefinition, IEditable, IEditable> interceptEditable = null, Action<RegistrationConventions> configureConventions = null)
		{
			var conventions = registration.DefaultConventions;
			var definition = registration.Definition;

			if (configureConventions != null)
				configureConventions(conventions);

			foreach (var container in conventions.Containers(definition))
			{
				registration.Register(container);
			}

			foreach (var pd in definition.Properties.Values)
			{
				if (pd.Editable != null)
					continue;

				var editable = conventions.Editable(pd);
				if (editable != null)
				{
					editable.Name = pd.Name;
					editable.ContainerName = conventions.EditableContainer(pd) ?? editable.ContainerName;

					if (interceptEditable != null)
						editable = interceptEditable(pd, editable);
					
					if (editable != null)
						registration.RegisterEditable(editable);
				}
			}

			conventions.Finalize(definition);
		}
	}
}
