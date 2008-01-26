using System;
using System.Collections.Generic;
using System.Security.Principal;
using MbUnit.Framework;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Persistence;
using N2.Tests.Definitions.Items;
using N2.Web.UI;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class DefinitionTests : TypeFindingBase
	{
		#region Setup

		private IPrincipal user;
		private DefaultDefinitionManager definitions;

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
					typeof (DefinitionAutoCreatedItem)
				};
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			user = mocks.DynamicMock<IPrincipal>();

			mocks.ReplayAll();

			EditableHierarchyBuilder<IEditable> hierarchyBuilder = new EditableHierarchyBuilder<IEditable>();
			DefinitionBuilder builder =
				new DefinitionBuilder(typeFinder, hierarchyBuilder, new AttributeExplorer<EditorModifierAttribute>(),
				                      new AttributeExplorer<IDisplayable>(), new AttributeExplorer<IEditable>(),
				                      new AttributeExplorer<IEditableContainer>());
			IItemNotifier notifier = mocks.DynamicMock<IItemNotifier>();
			mocks.Replay(notifier);
			definitions = new DefaultDefinitionManager(builder, notifier);
		}

		[TearDown]
		public void TearDown()
		{
			mocks.ReplayAll();
			mocks.VerifyAll();
		}

		#endregion

		[Test]
		public void CanCreateNewItemInstance()
		{
			DefinitionTextPage item = definitions.CreateInstance<DefinitionTextPage>(null);
			Assert.IsNotNull(item, "Couldn't create item");
		}

		[Test]
		public void TextPageHasRightZone()
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
		public void RightColumnTeaserAllowsRightZone()
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
		public void StartPageIsntAllowedBelowStartPage()
		{
			ItemDefinition startPageDef = definitions.GetDefinition(typeof (DefinitionStartPage));
			IList<ItemDefinition> childDefinitions = startPageDef.GetAllowedChildren(string.Empty, user);
			EnumerableAssert.DoesntContain(childDefinitions, startPageDef, "One of the start page's "+ childDefinitions.Count +" child definitions was the start page itself.");
		}

		[Test]
		public void TextPageIsAllowedBelowStartPage()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren(string.Empty, user);
			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextPage)));
		}

		[Test]
		public void AllowTeaserInStartPageRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren("Right", user);
			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		public void TeaserIsAllowedInTextPageRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren("Right", user);
			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		public void TextItemIsAllowedOnStartPageLeftAndCenterZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren("LeftAndCenter", user);
			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextItem)));
		}

		[Test]
		public void TeaserIsntAllowedOnStartPageLeftAndCenterZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren("LeftAndCenter", user);
			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionRightColumnTeaser)));
		}

		[Test]
		[ExpectedException(typeof (N2Exception))]
		public void AccessingAnUnexistantZoneThrowsException()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			definition.GetAllowedChildren("LeftAndCenter", user);
		}

		[Test]
		public void TeaserIsntAllowedWithoutZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionStartPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren(string.Empty, user);
			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionTextItem)));
		}

		[Test]
		public void NewsListIsAllowedOnNewsPageRightZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionNewsPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren("Right", user);
			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionNewsList)));
		}

		[Test]
		public void NewsListHasTwoAllowedZones()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionNewsList));
			EnumerableAssert.Contains(definition.AllowedZoneNames, "Right");
			EnumerableAssert.Contains(definition.AllowedZoneNames, "");
		}

		[Test]
		public void NewsListIsAllowedOnNewsPageEmptyZone()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionNewsPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren(string.Empty, user);
			EnumerableAssert.Contains(childDefinitions, definitions.GetDefinition(typeof (DefinitionNewsList)));
		}

		[Test]
		public void NewsListIsntAllowedOnTextPage()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			IList<ItemDefinition> childDefinitions = definition.GetAllowedChildren("Right", user);
			EnumerableAssert.DoesntContain(childDefinitions, definitions.GetDefinition(typeof (DefinitionNewsList)));
		}

		[Test]
		public void TextPageHasThreeEditable()
		{
			ItemDefinition definition = definitions.GetDefinition(typeof (DefinitionTextPage));
			IList<IEditable> editables = definition.Editables;
			Assert.AreEqual(3, editables.Count);
		}

		[Test]
		public void NoItemAttribute_DefaultsToTypeName()
		{
			Type itemType = typeof (DefinitionStartPage);
			ItemDefinition definition = definitions.GetDefinition(itemType);
			Assert.AreEqual(itemType.Name, definition.Discriminator);
		}

		[Test]
		public void ItemAttributeWithoutDiscriminator_DefaultsToTypeName()
		{
			Type itemType = typeof (DefinitionTextPage);
			ItemDefinition definition = definitions.GetDefinition(itemType);
			Assert.AreEqual(itemType.Name, definition.Discriminator);
		}

		[Test]
		public void InterfacesCanBeUsedToConstrainAllowedTypes()
		{
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof (DefinitionMenuItem));
			ItemDefinition startPageDefinition = definitions.GetDefinition(typeof (DefinitionStartPage));
			EnumerableAssert.Contains(startPageDefinition.AllowedChildren, menuDefinition);
		}

		[Test]
		public void ItemWithoutTheInterfaceIsntAllowed()
		{
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof (DefinitionMenuItem));
			ItemDefinition textPageDefinition = definitions.GetDefinition(typeof (DefinitionTextItem));
			EnumerableAssert.DoesntContain(textPageDefinition.AllowedChildren, menuDefinition);
		}

		[Test]
		public void NoAllowedZonesDefaultsToAllZonesAllowed()
		{
			ItemDefinition menuDefinition = definitions.GetDefinition(typeof (DefinitionMenuItem));
			bool isAllowed = menuDefinition.IsAllowedInZone("NotQuiteTheZoneYouExpectedPunk");
			Assert.IsTrue(isAllowed, "Wasn't allowed in the zone.");
		}

		[Test]
		public void ItemWithNoneAuthorizedIsntAllowed()
		{
			ItemDefinition autoDefinition = definitions.GetDefinition(typeof (DefinitionAutoCreatedItem));
			foreach (ItemDefinition definition in definitions.GetDefinitions())
			{
				IList<ItemDefinition> allowedDefinitions =
					definition.GetAllowedChildren(string.Empty, CreatePrincipal("admin", "Administrator"));
				EnumerableAssert.DoesntContain(allowedDefinitions, autoDefinition);
			}
		}
	}
}