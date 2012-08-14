using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Details;
using N2.Integrity;
using N2.Persistence;
using N2.Persistence.Proxying;
using N2.Security;
using N2.Tests.Definitions.Items;
using N2.Web;
using N2.Web.UI;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class DefinitionTests : TypeFindingBase
	{
		#region Setup

		private IPrincipal user;
		private DefinitionManager definitions;
		private ContentActivator activator;
		private DefinitionMap map;

		protected override Type[] GetTypes()
		{
			return new Type[]
			{
				typeof (DefinitionNewsList),
				typeof (DefinitionNewsPage),
				typeof (DefinitionRightColumnPart),
				typeof (DefinitionRightColumnTeaser),
				typeof (DefinitionStartPage),
				typeof (DefinitionTextItem),
				typeof (DefinitionTextPage),
				typeof (DefinitionTwoColumnPage),
				typeof (DefinitionMenuItem),
				typeof (DefinitionAutoCreatedItem),
				typeof (DefinitionDisabled),
				typeof (DefinitionReplaced),
				typeof (DefinitionDisablingReplacement),
				typeof (DefinitionReplacement),
				typeof (DefinitionOne),
				typeof (DefinitionTwo),
				typeof (DefinitionReplacesNumber1),
				typeof (DefinitionRemovesNumber2),
				typeof (DefinitionUndefined),
				typeof (DefinitionFreeItem),
				typeof (DefinitionControllingParent),
				typeof (DefinitionOppressedChild),
				typeof (DefinitionPartDefinedItem),
				typeof (DefinitionWithPersistable)
			};
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			user = CreatePrincipal("SomeSchmuck");

			map = new DefinitionMap();
			DefinitionBuilder builder = new DefinitionBuilder(map, typeFinder, new TransformerBase<IUniquelyNamed>[0], new EngineSection());
			IItemNotifier notifier = mocks.DynamicMock<IItemNotifier>();
			mocks.Replay(notifier);
			var changer = new N2.Edit.Workflow.StateChanger();
			activator = new ContentActivator(changer, notifier, new EmptyProxyFactory());
			definitions = new DefinitionManager(new[] { new DefinitionProvider(builder) }, new ITemplateProvider[0], activator, changer);
		}

		#endregion

		[Test, Obsolete]
		public void CanCreate_NewItemInstance()
		{
			DefinitionTextPage item = definitions.CreateInstance<DefinitionTextPage>(null);
			Assert.IsNotNull(item, "Couldn't create item");
		}

		[Test]
		public void TextPage_HasRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			IList<AvailableZoneAttribute> availableZones = definition.AvailableZones;
			EnumerableAssert.Contains(availableZones, new AvailableZoneAttribute("Right", "Right"));
		}

		[Test]
		public void StartPageHas_LeftAndCenter_AndRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			IList<AvailableZoneAttribute> availableZones = definition.AvailableZones;
			EnumerableAssert.Contains(availableZones, new AvailableZoneAttribute("Right", "Right"));
			EnumerableAssert.Contains(availableZones, new AvailableZoneAttribute("Left and Center", "LeftAndCenter"));
		}

		[Test]
		public void RightColumnTeaser_AllowsRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionRightColumnTeaser));
			IEnumerable<string> zones = definition.AllowedZoneNames;
			EnumerableAssert.Count(1, zones);
			EnumerableAssert.Contains(zones, "Right");
		}

		[Test]
		public void TextItemAllows_LeftAndCenter_AndRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextItem));
			IEnumerable<string> zones = definition.AllowedZoneNames;
			EnumerableAssert.Count(2, zones);
			EnumerableAssert.Contains(zones, "Right");
			EnumerableAssert.Contains(zones, "LeftAndCenter");
		}

		// DefinitionAutoCreatedItem
		// DefinitionMenuItem
		//		parents: ILeftColumnlPage
		// DefinitionNewsList
		//		parents: DefinitionNewsPage
		// DefinitionNewsPage
		// DefinitionRightColumnPart
		// DefinitionRightColumnTeaser
		// DefinitionStartPage: DefinitionTwoColumnPage, ILeftColumnlPage
		//		parents: none
		// DefinitionTextItem
		//		parents: DefinitionTwoColumnPage
		// DefinitionTextPage: DefinitionTwoColumnPage, ILeftColumnPage
		// DefinitionTwoColumnPage
		// ILeftColumnPage


		[Test]
		public void StartPage_IsntAllowed_BelowStartPage()
		{
			ItemDefinition startPageDef = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionStartPage(), string.Empty);

			EnumerableAssert.DoesntContain(childDefinitions, startPageDef, "One of the start page's child definitions was the start page itself.");
		}

		[Test]
		public void TextPage_IsAllowed_BelowStartPage()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionStartPage(), string.Empty);

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextPage)));
		}

		[Test]
		public void AllowTeaser_InStartPage_RightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionStartPage(), "Right");

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		public void Teaser_IsAllowed_InTextPage_RightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionTextPage(), "Right");

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		public void TextItem_IsAllowed_OnStartPage_LeftAndCenterZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionStartPage(), "LeftAndCenter");

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextItem)));
		}

		[Test]
		public void Teaser_IsntAllowed_OnStartPage_LeftAndCenterZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionStartPage(), "LeftAndCenter");

			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		public void Accessing_AnUnexistantZone_DoesntThrowException()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			definitions.GetAllowedChildren(new DefinitionTextPage(), "LeftAndCenter");
		}

		[Test]
		public void Teaser_IsntAllowed_WithoutZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionStartPage(), string.Empty);

			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextItem)));
		}

		[Test]
		public void NewsList_IsAllowed_OnNewsPageRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionNewsPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionNewsPage(), "Right");

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionNewsList)));
		}

		[Test]
		public void NewsList_HasTwo_AllowedZones()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionNewsList));
			EnumerableAssert.Contains(definition.AllowedZoneNames, "Right");
			EnumerableAssert.Contains(definition.AllowedZoneNames, "");
		}

		[Test]
		public void NewsList_IsAllowed_OnNewsPageEmptyZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionNewsPage));
			IEnumerable<ItemDefinition> childDefinitions = definitions.GetAllowedChildren(new DefinitionNewsPage(), string.Empty);
			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionNewsList)));
		}

		[Test]
		public void NewsList_IsntAllowed_OnTextPage()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			var childDefinitions = definitions.GetAllowedChildren(new DefinitionTextPage(), "Right");

			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionNewsList)));
		}

		[Test]
		public void TextPage_Has2InheritedEditables_AndOneAddedToProperty()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			IList<IEditable> editables = definition.Editables;
			Assert.AreEqual(3, editables.Count);
		}

		[Test]
		public void No_ItemAttribute_DefaultsToTypeName()
		{
			Type itemType = typeof (DefinitionStartPage);
			ItemDefinition definition = definitions.GetDefinition(itemType);
			Assert.AreEqual(itemType.Name, definition.Discriminator);
		}

		[Test]
		public void ItemAttribute_WithoutDiscriminator_DefaultsToTypeName()
		{
			Type itemType = typeof (DefinitionTextPage);
			ItemDefinition definition = definitions.GetDefinition(itemType);
			Assert.AreEqual(itemType.Name, definition.Discriminator);
		}

		[Test]
		public void Interfaces_CanBeUsedToConstrainAllowedTypes()
		{
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof(DefinitionMenuItem)); // RestrictParents: ILeftColumnlPage
			ItemDefinition startPageDefinition = definitions.GetDefinition(typeof(DefinitionStartPage)); // RestrictParents: None, implements ILeftColumnlPage

			Assert.That(startPageDefinition.GetAllowedChildren(definitions, null).Contains(menuDefinition));
		}

		[Test]
		public void Item_WithoutTheInterface_IsntAllowed()
		{
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof (DefinitionMenuItem));
			ItemDefinition textPageDefinition = definitions.GetDefinition(typeof (DefinitionTextItem));

			Assert.That(!textPageDefinition.GetAllowedChildren(definitions, null).Contains(menuDefinition));
		}

		[Test]
		public void NoAllowedZones_DefaultsTo_AllZonesAllowed()
		{
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof (DefinitionMenuItem));
			bool isAllowed = menuDefinition.IsAllowedInZone("NotQuiteTheZoneYouExpectedPunk");
			Assert.IsTrue(isAllowed, "Wasn't allowed in the zone.");
		}

		[Test]
		[Obsolete]
		public void ItemAllowedInNamedZones_IsNotAllowedIn_ZoneWithoutName()
		{
			var pageDefinition = definitions.GetDefinition(typeof(DefinitionTextPage));
			var freeDefinition = definitions.GetDefinition(typeof(DefinitionFreeItem));

			var allowedChildren = definitions.GetAllowedChildren(new DefinitionTextPage(), "", CreatePrincipal("admin", "Administrator"));

			Assert.That(allowedChildren.Contains(freeDefinition), Is.False);
		}

		[Test]
		public void ItemAllowedInNamedZones_IsAllowedIn_ZoneWithName()
		{
			var pageDefinition = definitions.GetDefinition(typeof(DefinitionTextPage));
			var freeDefinition = definitions.GetDefinition(typeof (DefinitionFreeItem));
			var allowedChildren = definitions.GetAllowedChildren(new DefinitionTextPage(), "SomeZone");
			
			Assert.That(allowedChildren.Contains(freeDefinition), Is.True);
		}

		[Test]
		public void Item_WithNoneAuthorized_IsntFilteredByDefault()
		{
			ItemDefinition autoDefinition = definitions.GetDefinition(typeof(DefinitionAutoCreatedItem));
			var autoDefinitionIsAllowedSomewhere = definitions.GetDefinitions()
				.SelectMany(d => definitions.GetAllowedChildren(Activator.CreateInstance(d.ItemType) as ContentItem, string.Empty))
				.Any(d => d == autoDefinition);
			
			Assert.That(autoDefinitionIsAllowedSomewhere);
		}

		[Test]
		public void Item_WithNoneAuthorized_CanBeFiltered()
		{
			ItemDefinition autoDefinition = definitions.GetDefinition(typeof(DefinitionAutoCreatedItem));
			foreach (ItemDefinition definition in definitions.GetDefinitions())
			{
				var parent = Activator.CreateInstance(definition.ItemType) as ContentItem;
				var allDefinitions = definitions.GetAllowedChildren(parent, string.Empty);
				var allowedDefinitions = allDefinitions.WhereAuthorized(new SecurityManager(new ThreadContext(), new EditSection()), user, parent);
				EnumerableAssert.DoesntContain(allowedDefinitions, autoDefinition);
			}
		}

		[Test]
		public void Definition_IsEnabledByDefault()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			Assert.IsTrue(definition.Enabled);
		}

		[Test]
		public void ReplacesParentDefinition_DisablesDefinitionOfParent()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionDisabled));
			Assert.IsFalse(definition.Enabled);
		}

		[Test]
		[Obsolete]
		public void ReplacingDefinition_ShowsUp_InAllowedChildDefinitions()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition replacingDefinition = definitions.GetDefinition(typeof(DefinitionDisablingReplacement));
			IList<ItemDefinition> allowedChildren = definitions.GetAllowedChildren(new DefinitionTextPage(), null, null);

			EnumerableAssert.Contains(allowedChildren, replacingDefinition);
		}

		[Test]
		public void ReplacingDefinition_RemovesReplaced()
		{
			ItemDefinition replacingDefinition = definitions.GetDefinition(typeof(DefinitionReplaced));

			Assert.That(replacingDefinition, Is.Null);
		}

		[Test]
		public void ReplacingDefinition_MaintiansOwnType()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition replacingDefinition = definitions.GetDefinition(typeof(DefinitionReplacement));

			Assert.That(replacingDefinition.ItemType, Is.EqualTo(typeof(DefinitionReplacement)));
		}

		[Test]
		public void ReplacingDefinition_AssumesParentDefinition_Discriminator()
		{
			ItemDefinition replacingDefinition = definitions.GetDefinition(typeof(DefinitionReplacement));

			Assert.That(replacingDefinition.Discriminator, Is.EqualTo("DefinitionReplaced"));
		}

		[Test]
		public void DisabledDefinition_DoesntShowUp_InAllowedChildDefinitions()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition replacedDefinition = definitions.GetDefinition(typeof(DefinitionDisabled));
			var allowedChildren = definitions.GetAllowedChildren(new DefinitionTextPage(), null);

			EnumerableAssert.DoesntContain(allowedChildren, replacedDefinition);
		}

		[Test]
		public void ReplaceDefinitionsAttribute_CanDisable_TheSuppliedDefinitions()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition definitionOne = definitions.GetDefinition(typeof(DefinitionOne));
			ItemDefinition definitionReplacement = definitions.GetDefinition(typeof(DefinitionReplacesNumber1));

			var allowedChildren = definitions.GetAllowedChildren(new DefinitionTextPage(), null);
			
			EnumerableAssert.DoesntContain(allowedChildren, definitionOne, "Definition one shouldn't be in the list since it isn't enabled");
			EnumerableAssert.Contains(allowedChildren, definitionReplacement);
		}

		[Test]
		[Obsolete]
		public void ReplaceDefinitionsAttribute_CanRemove_TheSuppliedDefinitions()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition definitionTwo = definitions.GetDefinition(typeof(DefinitionTwo));
			ItemDefinition definitionReplacement = definitions.GetDefinition(typeof(DefinitionRemovesNumber2));

			IList<ItemDefinition> allowedChildren = definitions.GetAllowedChildren(new DefinitionTextPage(), null, null);

			Assert.That(definitionTwo, Is.Null);
			EnumerableAssert.Contains(allowedChildren, definitionReplacement);
		}

		[Test]
		public void Undefined_ContentItem_IsNotEnabled()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionUndefined));

			definition.ShouldBe(null);
		}

		[Test]
		public void Undefined_ContentItem_IsNotAllowedAsChild()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition undefinedDefinition = definitions.GetDefinition(typeof(DefinitionUndefined));

			var allowedChildren = definitions.GetAllowedChildren(new DefinitionTextPage(), null);

			EnumerableAssert.DoesntContain(allowedChildren, undefinedDefinition);
		}

		[Test]
		public void Item_WithEmptyDefinition_DoesGetADefaultTitle()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionStartPage));

			Assert.IsNotNull(definition.Title);
			Assert.IsNotEmpty(definition.Title);
		}

		[Test, Obsolete]
		public void Item_Inherits_AllowedReaders_FromParent()
		{
			var enforcer = new SecurityEnforcer(persister, new SecurityManager(new ThreadContext(), new EditSection()), activator, MockRepository.GenerateStub<IUrlParser>(), new ThreadContext());
			enforcer.Start();
			
			DefinitionTextPage page = definitions.CreateInstance<DefinitionTextPage>(null);
			page.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(page, "Administrators"));

			try
			{
				DefinitionTextPage child = definitions.CreateInstance<DefinitionTextPage>(page);

				Assert.That(child.AuthorizedRoles.Count, Is.EqualTo(1));
				Assert.That(child.AuthorizedRoles[0].Role, Is.EqualTo("Administrators"));
				Assert.That(child.AuthorizedRoles[0].EnclosingItem, Is.EqualTo(child));
			}
			finally
			{
				enforcer.Stop();
			}
		}

		[Test, Obsolete]
		public void Item_Inherits_AllowedEditors_FromParent()
		{
			var enforcer = new SecurityEnforcer(persister, new SecurityManager(new ThreadContext(), new EditSection()), activator, MockRepository.GenerateStub<IUrlParser>(), new ThreadContext());
			enforcer.Start();

			DefinitionTextPage page = definitions.CreateInstance<DefinitionTextPage>(null);
			DynamicPermissionMap.SetRoles(page, Permission.Publish, new string[] { "Group1" });

			try
			{
				DefinitionTextPage child = definitions.CreateInstance<DefinitionTextPage>(page);

				Assert.That(DynamicPermissionMap.GetRoles(child, Permission.Publish).Count(), Is.EqualTo(1));
				Assert.That(DynamicPermissionMap.GetRoles(child, Permission.Publish).Contains("Group1"));
			}
			finally
			{
				enforcer.Stop();
			}
		}

		[Test]
		public void ItemDefinition_CanOverride_EditableAttribute_OnParent()
		{
			var newsDefinition = definitions.GetDefinition(typeof (DefinitionNewsPage));
			var textEditables = newsDefinition.Editables.Where(e => e.Name == "Text");
			var editable = textEditables.First();

			Assert.That(textEditables.Count(), Is.EqualTo(1));
			Assert.That(editable.Title, Is.EqualTo("Plain Text"));
			Assert.That(editable.SortOrder, Is.EqualTo(200));
			Assert.That(editable, Is.TypeOf(typeof(EditableTextAttribute)));
		}

		[Test]
		public void CanRestrictChildren()
		{
			var parentDefinition = definitions.GetDefinition(typeof(DefinitionControllingParent));
			var childDefinition = definitions.GetDefinition(typeof(DefinitionOppressedChild));
			var allowed = parentDefinition.GetAllowedChildren(definitions, null).ToList();
			Assert.That(allowed.Single(), Is.EqualTo(childDefinition));
		}

		[Test]
		public void PartDefinition_Defaults_AllowedZones_To_AllNamed()
		{
			var definition = definitions.GetDefinition(typeof(DefinitionPartDefinedItem));

			Assert.That(definition.AllowedIn, Is.EqualTo(AllowedZones.AllNamed));
		}

		[Test]
		public void Displayable_TakesPrecedence_OverEditable_WhenDefined()
		{
			var definition = definitions.GetDefinition(typeof(DefinitionTextPage));

			var displayable = definition.Displayables.Single(d => d.Name == "Text");
			Assert.That(displayable, Is.InstanceOf<DisplayableLiteralAttribute>());
		}

		[Test]
		public void DefinitionProperties_AreGenerated()
		{
			var definition = definitions.GetDefinition(typeof(DefinitionWithPersistable));
			var pd = definition.Properties["Hello"];
			pd.Name.ShouldBe("Hello");
			pd.Persistable.PersistAs.ShouldBe(PropertyPersistenceLocation.Column);
			pd.Info.DeclaringType.ShouldBe(typeof(DefinitionWithPersistable));
			pd.PropertyType.ShouldBe(typeof(string));
		}

		[TabContainer("tab1")]
		public class ParentContentItem : ContentItem
		{
		}
		[TabContainer("tab2")]
		public class ChildContentItem : ParentContentItem
		{
		}

		[Test]
		public void Containerns_AreInherited()
		{
			var d = map.GetOrCreateDefinition(typeof(ChildContentItem));
			d.Containers.Count.ShouldBe(2);
		}



		[SidebarContainer("Metadata", 10, HeadingText = "Metadata")]
		[SidebarContainer("Navigation", 20, HeadingText = "Navigation")]
		[TabContainer("CurrentSetting", "Current page setting", 30)]
		[TabContainer("Content", "Content", 0)]
		[TabContainer("PageSettings", "Page settings", 30)]
		[TabContainer("SiteResources", "Site resources", 20)]
		public abstract class BasePage : ContentItem
		{
			[EditableText(ContainerName = "Metadata")]
			public override string Title
			{
				get { return base.Title; }
				set { base.Title = value; }
			}

			[EditableText(ContainerName = "Content")]
			public virtual string Heading { set; get; }

			[EditableText(ContainerName = "Metadata")]
			public virtual string SEOTitle { get; set; }

			[EditableText(ContainerName = "Metadata")]
			public virtual string MetaDescription { set; get; }

			[EditableText(ContainerName = "Metadata")]
			public virtual string MetaKeywords { set; get; }

			[EditableUrl(ContainerName = "CurrentSetting")]
			public virtual string RedirectURL { set; get; }

			[EditableCheckBox(ContainerName = "Navigation")]
			public virtual bool IsSubNavigationRoot { set; get; }

			[EditableText(ContainerName = "Navigation")]
			public virtual string NavigationName { set; get; }
		}

		[SidebarContainer("CategoryInfo", 0)]
		public abstract partial class SpecificPage : BasePage
		{
			[EditableNumber(ContainerName = "CategoryInfo")]
			public virtual int CategoryItemId { get; set; }
		}

		[Test]
		public void Editors_CanBeAddedTo_ContainerInBaseType()
		{
			var d = map.GetOrCreateDefinition(typeof(SpecificPage));
			d.Containers.Count.ShouldBe(7);
			d.Editables.Count.ShouldBe(9);
			d.Editables["Title"].ContainerName.ShouldBe("Metadata");
			d.Editables["Heading"].ContainerName.ShouldBe("Content");
			d.Editables["SEOTitle"].ContainerName.ShouldBe("Metadata");
			d.Editables["MetaDescription"].ContainerName.ShouldBe("Metadata");
			d.Editables["MetaKeywords"].ContainerName.ShouldBe("Metadata");
			d.Editables["RedirectURL"].ContainerName.ShouldBe("CurrentSetting");
			d.Editables["IsSubNavigationRoot"].ContainerName.ShouldBe("Navigation");
			d.Editables["NavigationName"].ContainerName.ShouldBe("Navigation");
			d.Editables["CategoryItemId"].ContainerName.ShouldBe("CategoryInfo");
		}
	}
}