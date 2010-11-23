using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Principal;
using N2.Configuration;
using NUnit.Framework;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Persistence;
using N2.Tests.Definitions.Items;
using N2.Tests.Fakes;
using N2.Persistence.Proxying;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class DefinitionTests : TypeFindingBase
	{
		#region Setup

		private IPrincipal user;
		private DefinitionManager definitions;

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
				typeof (DefinitionReplaced),
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
				typeof (DefinitionRemovesParent),
				typeof (DefinitionRemovedByParent)
			};
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			user = CreatePrincipal("SomeSchmuck");

			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, new EngineSection(), new FakeEditUrlManager());
			IItemNotifier notifier = mocks.DynamicMock<IItemNotifier>();
			mocks.Replay(notifier);
			definitions = new DefinitionManager(builder, new N2.Edit.Workflow.StateChanger(), notifier, new EmptyProxyFactory());
		}

		#endregion

		[Test]
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
			var childDefinitions = definitions.GetAllowedChildren(startPageDef, string.Empty, user);

			EnumerableAssert.DoesntContain(childDefinitions, startPageDef, "One of the start page's child definitions was the start page itself.");
		}

		[Test]
		public void TextPage_IsAllowed_BelowStartPage()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(definition, string.Empty, user);

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextPage)));
		}

		[Test]
		public void AllowTeaser_InStartPage_RightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(definition, "Right", user);

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		public void Teaser_IsAllowed_InTextPage_RightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			var childDefinitions = definitions.GetAllowedChildren(definition, "Right", user);

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		public void TextItem_IsAllowed_OnStartPage_LeftAndCenterZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(definition, "LeftAndCenter", user);

			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextItem)));
		}

		[Test]
		public void Teaser_IsntAllowed_OnStartPage_LeftAndCenterZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(definition, "LeftAndCenter", user);

			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		public void Accessing_AnUnexistantZone_DoesntThrowException()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			definitions.GetAllowedChildren(definition, "LeftAndCenter", user);
		}

		[Test]
		public void Teaser_IsntAllowed_WithoutZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			var childDefinitions = definitions.GetAllowedChildren(definition, string.Empty, user);

			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextItem)));
		}

		[Test]
		public void NewsList_IsAllowed_OnNewsPageRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionNewsPage));
			var childDefinitions = definitions.GetAllowedChildren(definition, "Right", user);

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
			IEnumerable<ItemDefinition> childDefinitions = definitions.GetAllowedChildren(definition, string.Empty, user);
			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionNewsList)));
		}

		[Test]
		public void NewsList_IsntAllowed_OnTextPage()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			var childDefinitions = definitions.GetAllowedChildren(definition, "Right", user);

			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionNewsList)));
		}

		[Test]
		public void TextPage_HasThreeEditables()
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
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof (DefinitionMenuItem));
			ItemDefinition startPageDefinition = definitions.GetDefinition(typeof (DefinitionStartPage));

			EnumerableAssert.Contains(startPageDefinition.AllowedChildren, menuDefinition);
		}

		[Test]
		public void Item_WithoutTheInterface_IsntAllowed()
		{
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof (DefinitionMenuItem));
			ItemDefinition textPageDefinition = definitions.GetDefinition(typeof (DefinitionTextItem));

			EnumerableAssert.DoesntContain(textPageDefinition.AllowedChildren, menuDefinition);
		}

		[Test]
		public void NoAllowedZones_DefaultsTo_AllZonesAllowed()
		{
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof (DefinitionMenuItem));
			bool isAllowed = menuDefinition.IsAllowedInZone("NotQuiteTheZoneYouExpectedPunk");
			Assert.IsTrue(isAllowed, "Wasn't allowed in the zone.");
		}

		[Test]
		public void ItemAllowedInNamedZones_IsNotAllowedIn_ZoneWithoutName()
		{
			var pageDefinition = definitions.GetDefinition(typeof(DefinitionTextPage));
			var freeDefinition = definitions.GetDefinition(typeof(DefinitionFreeItem));

			var allowedChildren = definitions.GetAllowedChildren(pageDefinition, "", CreatePrincipal("admin", "Administrator"));

			Assert.That(allowedChildren.Contains(freeDefinition), Is.False);
		}

		[Test]
		public void ItemAllowedInNamedZones_IsAllowedIn_ZoneWithName()
		{
			var pageDefinition = definitions.GetDefinition(typeof(DefinitionTextPage));
			var freeDefinition = definitions.GetDefinition(typeof (DefinitionFreeItem));
			var allowedChildren = definitions.GetAllowedChildren(pageDefinition, "SomeZone", user);
			
			Assert.That(allowedChildren.Contains(freeDefinition), Is.True);
		}

		[Test]
		public void Item_WithNoneAuthorized_IsntAllowed()
		{
			ItemDefinition autoDefinition = definitions.GetDefinition(typeof (DefinitionAutoCreatedItem));
			foreach (ItemDefinition definition in definitions.GetDefinitions())
			{
				IEnumerable<ItemDefinition> allowedDefinitions =
					definitions.GetAllowedChildren(definition, string.Empty, user);
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
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionReplaced));
			Assert.IsFalse(definition.Enabled);
		}

		[Test]
		public void ReplacingDefinition_ShowsUp_InAllowedChildDefinitions()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition replacingDefinition = definitions.GetDefinition(typeof(DefinitionReplacement));
			IList<ItemDefinition> allowedChildren = definitions.GetAllowedChildren(definition, null, null);

			EnumerableAssert.Contains(allowedChildren, replacingDefinition);
		}

		[Test]
		public void DisabledDefinition_DoesntShowUp_InAllowedChildDefinitions()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition replacedDefinition = definitions.GetDefinition(typeof(DefinitionReplaced));
			IList<ItemDefinition> allowedChildren = definitions.GetAllowedChildren(definition, null, null);

			EnumerableAssert.DoesntContain(allowedChildren, replacedDefinition);
		}

		[Test]
		public void ReplaceDefinitionsAttribute_CanDisable_TheSuppliedDefinitions()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition definitionOne = definitions.GetDefinition(typeof(DefinitionOne));
			ItemDefinition definitionReplacement = definitions.GetDefinition(typeof(DefinitionReplacesNumber1));

			IList<ItemDefinition> allowedChildren = definitions.GetAllowedChildren(definition, null, null);
			
			EnumerableAssert.DoesntContain(allowedChildren, definitionOne, "Definition one shouldn't be in the list since it isn't enabled");
			EnumerableAssert.Contains(allowedChildren, definitionReplacement);
		}

		[Test]
		public void ReplaceDefinitionsAttribute_CanRemove_TheSuppliedDefinitions()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition definitionTwo = definitions.GetDefinition(typeof(DefinitionTwo));
			ItemDefinition definitionReplacement = definitions.GetDefinition(typeof(DefinitionRemovesNumber2));

			IList<ItemDefinition> allowedChildren = definitions.GetAllowedChildren(definition, null, null);

			Assert.That(definitionTwo, Is.Null);
			EnumerableAssert.Contains(allowedChildren, definitionReplacement);
		}

		[Test]
		public void Undefined_ContentItem_IsNotEnabled()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionUndefined));

			Assert.IsFalse(definition.IsDefined);
		}

		[Test]
		public void Undefined_ContentItem_IsNotAllowedAsChild()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionTextPage));
			ItemDefinition undefinedDefinition = definitions.GetDefinition(typeof(DefinitionUndefined));

			IList<ItemDefinition> allowedChildren = definitions.GetAllowedChildren(definition, null, null);

			EnumerableAssert.DoesntContain(allowedChildren, undefinedDefinition);
		}

		[Test]
		public void Item_WithEmptyDefinition_DoesGetADefaultTitle()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof(DefinitionStartPage));

			Assert.IsNotNull(definition.Title);
			Assert.IsNotEmpty(definition.Title);
		}

		[Test]
		public void Item_Inherits_AllowedRoles_FromParent()
		{
			DefinitionTextPage page = definitions.CreateInstance<DefinitionTextPage>(null);
			page.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(page, "Administrators"));

			DefinitionTextPage child = definitions.CreateInstance<DefinitionTextPage>(page);
			Assert.That(child.AuthorizedRoles.Count, Is.EqualTo(1));
			Assert.That(child.AuthorizedRoles[0].Role, Is.EqualTo("Administrators"));
			Assert.That(child.AuthorizedRoles[0].EnclosingItem, Is.EqualTo(child));
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
			Assert.That(editable, Is.TypeOf(typeof(EditableTextBoxAttribute)));
		}

		[Test]
		public void CanRestrictChildren()
		{
			var parentDefinition = definitions.GetDefinition(typeof(DefinitionControllingParent));
			var childDefinition = definitions.GetDefinition(typeof(DefinitionOppressedChild));

			Assert.That(parentDefinition.AllowedChildren.Contains(childDefinition));
			Assert.That(parentDefinition.AllowedChildren.Count, Is.EqualTo(1));
		}

		[Test]
		public void PartDefinition_Defaults_AllowedZones_To_AllNamed()
		{
			var definition = definitions.GetDefinition(typeof(DefinitionPartDefinedItem));

			Assert.That(definition.AllowedIn, Is.EqualTo(AllowedZones.AllNamed));
		}

		[Test]
		public void ReplacesParentDefinition_Removes_ParentsDefinition()
		{
			int count = definitions.GetDefinitions()
				.Where(d => d.ItemType == typeof(DefinitionRemovedByParent))
				.Count();

			Assert.That(count, Is.EqualTo(0));
		}

		[Test]
		public void ReplacesParentDefinition_Assumes_ParentsDefinition_Discriminator()
		{
			var definition = definitions.GetDefinition(typeof(DefinitionRemovesParent));

			Assert.That(definition.Discriminator, Is.EqualTo("DefinitionRemovedByParent"));
		}
	}
}