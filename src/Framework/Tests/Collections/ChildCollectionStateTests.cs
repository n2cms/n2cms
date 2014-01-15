using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence.Behaviors;
using Shouldly;
using N2.Collections;
using N2.Definitions;
using N2.Engine.Globalization;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class ChildCollectionStateTests : ItemPersistenceMockingBase
    {
        private BehaviorInvoker invoker;
        private N2.Definitions.Static.DefinitionMap map;

        private FirstItem root;
        
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            map = new N2.Definitions.Static.DefinitionMap();
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            root = CreateOneItem<FirstItem>(0, "root", null);

            invoker = new BehaviorInvoker(persister, map);
            invoker.Start();
        }

        // adding

        [Test]
        public void ContainsNavigatablePages_IsSet_ForPublicVisiblePages()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
        }

        [Test]
        public void ContainsHiddenPublicPages_IsSet_ForPublicInvisiblePages()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            child1.Visible = false;

            persister.Save(child1);

            root.ChildState.ShouldBe(CollectionState.ContainsHiddenPublicPages);
        }

        [Test]
        public void ContainsSecuredPages_IsSet_ForPagesWithAlteredPermission()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            child1.AlteredPermissions = N2.Security.Permission.Read;

            persister.Save(child1);

            root.ChildState.ShouldBe(CollectionState.ContainsVisibleSecuredPages);
        }

        [Test]
        public void ContainsSecuredPages_IsSet_ForInvisiblePagesWithAlteredPermission()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            child1.Visible = false;
            child1.AlteredPermissions = N2.Security.Permission.Read;

            persister.Save(child1);

            root.ChildState.ShouldBe(CollectionState.ContainsHiddenSecuredPages);
        }

        [Test]
        public void IsEmpty_IsSet_WhenNoChildren()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            child1.ChildState.ShouldBe(CollectionState.IsEmpty);
        }

        [Test]
        public void Combination_OfChildrenStates_Pages()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            var child2 = CreateOneItem<FirstItem>(0, "child2", root);
            child2.Visible = false;
            persister.Save(child2);

            var child3 = CreateOneItem<FirstItem>(0, "child3", root);
            child3.AlteredPermissions = N2.Security.Permission.Read;
            persister.Save(child3);

            var child4 = CreateOneItem<FirstItem>(0, "child4", root);
            child4.AlteredPermissions = N2.Security.Permission.Read;
            child4.Visible = false;
            persister.Save(child4);

            root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages | CollectionState.ContainsVisibleSecuredPages | CollectionState.ContainsHiddenPublicPages | CollectionState.ContainsHiddenSecuredPages);
        }

        [Test]
        public void ContainsPublicParts_IsSet_ForNonAuthorized_Parts()
        {
            var child2 = CreateOneItem<Items.FirstPart>(0, "child2", root);
            child2.ZoneName = "TheZone";
            persister.Save(child2);

            root.ChildState.ShouldBe(CollectionState.ContainsPublicParts);
        }

        [Test]
        public void ContainsSecuredParts_IsSet_ForAuthorized_Parts()
        {
            var child2 = CreateOneItem<Items.FirstPart>(0, "child2", root);
            child2.ZoneName = "TheZone";
            child2.AlteredPermissions = N2.Security.Permission.Read;
            persister.Save(child2);

            root.ChildState.ShouldBe(CollectionState.ContainsSecuredParts);
        }

        [Test]
        public void Combination_OfChildrenStates_PagesAndParts()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            var child2 = CreateOneItem<FirstItem>(0, "child2", root);
            child2.Visible = false;
            persister.Save(child2);

            var child3 = CreateOneItem<FirstItem>(0, "child3", root);
            child3.AlteredPermissions = N2.Security.Permission.Read;
            persister.Save(child3);

            var child4 = CreateOneItem<FirstItem>(0, "child4", root);
            child4.AlteredPermissions = N2.Security.Permission.Read;
            child4.Visible = false;
            persister.Save(child4);

            var child5 = CreateOneItem<Items.FirstPart>(0, "child5", root);
            child5.ZoneName = "TheZone";
            persister.Save(child5);

            var child6 = CreateOneItem<Items.FirstPart>(0, "child6", root);
            child6.ZoneName = "TheZone";
            child6.AlteredPermissions = N2.Security.Permission.Read;
            persister.Save(child6);

            root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages | CollectionState.ContainsVisibleSecuredPages | CollectionState.ContainsHiddenPublicPages | CollectionState.ContainsHiddenSecuredPages | CollectionState.ContainsPublicParts | CollectionState.ContainsSecuredParts);
        }

        [Test]
        public void Combination_OfChildrenStates_PagesAndParts_ForLargeCollection()
        {
            using (new Scope(() => SyncChildCollectionStateAttribute.LargeCollecetionThreshold = 100))
            {
                SyncChildCollectionStateAttribute.LargeCollecetionThreshold = 1;

                persister.Save(new FirstItem { Name = "child0", Parent = root }); // persist first to invoke added

                var child1 = CreateOneItem<FirstItem>(0, "child1", root);
                persister.Save(child1);

                var child2 = CreateOneItem<FirstItem>(0, "child2", root);
                child2.Visible = false;
                persister.Save(child2);

                var child3 = CreateOneItem<FirstItem>(0, "child3", root);
                child3.AlteredPermissions = N2.Security.Permission.Read;
                persister.Save(child3);

                var child4 = CreateOneItem<FirstItem>(0, "child4", root);
                child4.AlteredPermissions = N2.Security.Permission.Read;
                child4.Visible = false;
                persister.Save(child4);

                var child5 = CreateOneItem<Items.FirstPart>(0, "child5", root);
                child5.ZoneName = "TheZone";
                persister.Save(child5);

                var child6 = CreateOneItem<Items.FirstPart>(0, "child6", root);
                child6.ZoneName = "TheZone";
                child6.AlteredPermissions = N2.Security.Permission.Read;
                persister.Save(child6);

                root.ChildState.ShouldBe(CollectionState.IsLarge | CollectionState.ContainsVisiblePublicPages | CollectionState.ContainsVisibleSecuredPages | CollectionState.ContainsHiddenPublicPages | CollectionState.ContainsHiddenSecuredPages | CollectionState.ContainsPublicParts | CollectionState.ContainsSecuredParts);
            }
        }

        [Test]
        public void ChangingItemState_AffectsCollectionState()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            child1.Visible = false;
            persister.Save(child1);

            root.ChildState.ShouldBe(CollectionState.ContainsHiddenPublicPages);
        }

        [Test]
        public void ChangingItemState_AffectsCollectionState_WithoutRemoving_StateInducedByOtherSiblings()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);
            persister.Save(CreateOneItem<FirstItem>(0, "child2", root));

            child1.Visible = false;
            persister.Save(child1);

            root.ChildState.ShouldBe(CollectionState.ContainsHiddenPublicPages | CollectionState.ContainsVisiblePublicPages);
        }

        // removing

        [Test]
        public void RemovingItem_ReduceToLeafNode()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);
            persister.Delete(child1);

            root.ChildState.ShouldBe(CollectionState.IsEmpty);
        }

        [Test]
        public void RemovingItem_ReduceToHiddenState()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            var child2 = CreateOneItem<FirstItem>(0, "child2", root);
            child2.Visible = false;
            persister.Save(child2);

            persister.Delete(child1);

            root.ChildState.ShouldBe(CollectionState.ContainsHiddenPublicPages);
        }

        [Test]
        public void RemovingItem_ReduceToCombinedState()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            var child2 = CreateOneItem<FirstItem>(0, "child2", root);
            child2.Visible = false;
            persister.Save(child2);

            var child3 = CreateOneItem<FirstItem>(0, "child3", root);
            child3.AlteredPermissions = N2.Security.Permission.Read;
            persister.Save(child3);

            var child4 = CreateOneItem<FirstItem>(0, "child4", root);
            child4.AlteredPermissions = N2.Security.Permission.Read;
            child4.Visible = false;
            persister.Save(child4);

            var child5 = CreateOneItem<Items.FirstPart>(0, "child5", root);
            child5.ZoneName = "TheZone";
            persister.Save(child5);

            var child6 = CreateOneItem<Items.FirstPart>(0, "child6", root);
            child6.ZoneName = "TheZone";
            child6.AlteredPermissions = N2.Security.Permission.Read;
            persister.Save(child6);

            persister.Delete(child3);
            persister.Delete(child4);
            persister.Delete(child5);

            root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages | CollectionState.ContainsHiddenPublicPages | CollectionState.ContainsSecuredParts);
        }

        [Test]
        public void RemovingItem_MaiainState_IfEnforcedByOtherItem()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            var child2 = CreateOneItem<FirstItem>(0, "child2", root);
            persister.Save(child2);

            persister.Delete(child1);

            root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
        }

        [Test]
        public void MovingItem_AddsStateToDestination()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            var child2 = CreateOneItem<FirstItem>(0, "child2", root);
            persister.Save(child2);

            persister.Move(child1, child2);

            child2.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
        }

        [Test]
        public void MovingItem_RemovesState_FromSource()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            child1.Visible = false;
            persister.Save(child1);

            var child2 = CreateOneItem<FirstItem>(0, "child2", root);
            persister.Save(child2);

            persister.Move(child1, child2);

            root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
            child2.ChildState.ShouldBe(CollectionState.ContainsHiddenPublicPages);
        }

        [Test]
        public void CopyingItem_AddsState_ToDestination()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            persister.Save(child1);

            var child2 = CreateOneItem<FirstItem>(0, "child2", root);
            persister.Save(child2);

            persister.Copy(child1, child2);

            child2.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
        }

        [Test]
        public void IsLarge_IsNotAdded_ForSmallChildCollections()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            try
            {
                SyncChildCollectionStateAttribute.LargeCollecetionThreshold = 3;

                persister.Save(child1);

                var child2 = CreateOneItem<FirstItem>(0, "child2", root);
                persister.Save(child2);

                root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
            }
            finally
            {
                SyncChildCollectionStateAttribute.LargeCollecetionThreshold = 100;
            }
        }

        [Test]
        public void IsLarge_IsAdded_ForChildCollections_WithCound_ReachesTreshold()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            try
            {
                SyncChildCollectionStateAttribute.LargeCollecetionThreshold = 3;

                persister.Save(child1);
                persister.Save(new FirstItem { Parent = root, State = ContentState.Published });
                persister.Save(new FirstItem { Parent = root, State = ContentState.Published });

                root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages | CollectionState.IsLarge);
            }
            finally
            {
                SyncChildCollectionStateAttribute.LargeCollecetionThreshold = 100;
            }
        }

        [Test]
        public void IsLarge_IsRemoved_ForChildCollections_WhenCount_DecreasesBelowTreshold()
        {
            var child1 = CreateOneItem<FirstItem>(0, "child1", root);
            try
            {
                SyncChildCollectionStateAttribute.LargeCollecetionThreshold = 3;

                persister.Save(child1);
                persister.Save(new FirstItem { Parent = root, State = ContentState.Published });
                persister.Save(new FirstItem { Parent = root, State = ContentState.Published });

                persister.Delete(child1);

                root.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
            }
            finally
            {
                SyncChildCollectionStateAttribute.LargeCollecetionThreshold = 100;
            }
        }
    }
}
