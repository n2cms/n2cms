using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Configuration;
using N2.Definitions;
using N2.Security;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Definitions
{
    [TestFixture]
    public class EditableExplorerTests : ItemTestsBase
    {
        EditableHierarchyBuilder hierarchyBuilder = new EditableHierarchyBuilder(new SecurityManager(new ThreadContext(), new EditSection()), TestSupport.SetupEngineSection());
        AttributeExplorer explorer = new AttributeExplorer();



        [Test]
        public void FindsDetailDefinedOnClass()
        {
            IList<IEditable> editables = explorer.Find<IEditable>(typeof(Items.DefinitionTwoColumnPage));
            Assert.AreEqual(2, editables.Count);
            TypeAssert.AreEqual(typeof(N2.Details.WithEditableTitleAttribute), editables[0]);
            TypeAssert.AreEqual(typeof(N2.Details.WithEditableNameAttribute), editables[1]);
        }

        [Test]
        public void FindsDetailsDefinedOnClassAndOnProperty()
        {
            IList<IEditable> editables = explorer.Find<IEditable>(typeof(Items.DefinitionTextPage));
            Assert.AreEqual(3, editables.Count);
        }

        [Test]
        public void FindsContainers()
        {
            IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(typeof(Items.DefinitionTextPage));
            Assert.AreEqual(2, containers.Count);
        }

        [Test]
        public void RedefinedContainerIsntDuplicated()
        {
            IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(typeof(Items.DefinitionNewsPage));
            Assert.AreEqual(2, containers.Count);
        }

        //  - root
        //      - name
        //      - fieldset
        //          - title
        //          - fieldset
        //              - freetextarea
        [Test]
        public void CanBuildContainerAndEditableStructure()
        {
            Type itemType = typeof(Items.DefinitionTextPage);
            IList<IEditable> editables = explorer.Find<IEditable>(itemType);
            IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(itemType);
            HierarchyNode<IContainable> rootContainer = hierarchyBuilder.Build(containers.OfType<IContainable>(), editables.OfType<IContainable>());

            var contained = rootContainer.Children;
            Assert.AreEqual(2, contained.Count);

            Assert.AreEqual(typeof(N2.Details.WithEditableNameAttribute), contained[0].Current.GetType());

            var fieldSet = contained[1] as HierarchyNode<IContainable>;
            Assert.IsNotNull(fieldSet);
            Assert.AreEqual(typeof(N2.Web.UI.FieldSetContainerAttribute), fieldSet.Current.GetType());

            var containedByFieldSet = fieldSet.Children;//.GetContained(null);
            Assert.AreEqual(typeof(N2.Details.WithEditableTitleAttribute), containedByFieldSet[0].Current.GetType());

            var innerFieldSet = containedByFieldSet[1];
            Assert.IsNotNull(innerFieldSet);
            Assert.AreEqual(typeof(N2.Web.UI.FieldSetContainerAttribute), innerFieldSet.Current.GetType());

            var containedByInnerFieldSet = innerFieldSet.Children;//.GetContained(null);
            Assert.AreEqual(typeof(N2.Details.EditableFreeTextAreaAttribute), containedByInnerFieldSet[0].Current.GetType());
        }

        private void Print(HierarchyNode<IContainable> current)
        {
            System.Diagnostics.Debug.WriteLine(current.Current);
            System.Diagnostics.Debug.Indent();
            foreach (var child in current.Children)
                Print(child);
            System.Diagnostics.Debug.Unindent();
        }



        //0: inside3
        //1: first
        //2: inside1
        //3: inside2
        //4: inside1_1
        [Test]
        public void CanBuildHierarchy()
        {
            Type itemType = typeof(N2.Tests.Definitions.Definitions.ItemWithNestedContainers);
            IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(itemType);
            IList<IEditable> editables = explorer.Find<IEditable>(itemType);
            var rootContainer = hierarchyBuilder.Build(containers.OfType<IContainable>(), editables.OfType<IContainable>());

            var property0 = rootContainer.Children[0];//.GetContained(null)[0];
            Assert.AreSame(property0.Current, editables[0]);

            var first = rootContainer.Children[1];//.GetContained(null)[1] as IEditableContainer;
            Assert.AreSame(first.Current, containers[1]);

            var property1 = first.Children[1];//.GetContained(null)[1];
            Assert.AreSame(property1.Current, editables[1]);

            var inside1 = first.Children[2];//.GetContained(null)[2] as IEditableContainer;
            Assert.AreEqual(inside1.Current, containers[2]);

            var property2 = inside1.Children[0];//.GetContained(null)[0];
            Assert.AreSame(property2.Current, editables[2]);

            var inside1_1 = inside1.Children[1];//.GetContained(null)[1];
            Assert.AreEqual(inside1_1.Current, containers[4]);
        }

        [Test]
        public void ContainersAreNestedAndSorted()
        {
            Type itemType = typeof(Definitions.ItemWithNestedContainers);
            IList<IEditable> editables = new List<IEditable>();
            IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(itemType);
            HierarchyNode<IContainable> rootContainer = hierarchyBuilder.Build(containers.OfType<IContainable>(), editables.OfType<IContainable>());

            var first = rootContainer.Children[0];//.GetContained(null)[0] as IEditableContainer;
            Assert.IsNotNull(first.Current);
            Assert.AreSame(first.Current, containers[1]);
            Assert.AreEqual(3, first.Children.Count);//.GetContained(null).Count);
            
            var inside3 = first.Children[0];//.GetContained(null)[0] as IEditableContainer;
            Assert.IsNotNull(inside3.Current);
            Assert.AreSame(inside3.Current, containers[0]);
            Assert.AreEqual(0, inside3.Children.Count);//.GetContained(null).Count);

            var inside1 = first.Children[1];//.GetContained(null)[1] as IEditableContainer;
            Assert.IsNotNull(inside1.Current);
            Assert.AreSame(inside1.Current, containers[2]);
            Assert.AreEqual(1, inside1.Children.Count);//.GetContained(null).Count);

            var inside2 = first.Children[2];//.GetContained(null)[2] as IEditableContainer;
            Assert.IsNotNull(inside2.Current);
            Assert.AreSame(inside2.Current, containers[3]);
            Assert.AreEqual(0, inside2.Children.Count);//.GetContained(null).Count);
        }

        [Test]
        public void InvalidContainerReference_IsIgnored()
        {
            Type itemType = typeof(N2.Tests.Definitions.Definitions.ItemWithNestedContainers);
            IList<IEditable> editables = new List<IEditable>();
            IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(itemType);
            containers.RemoveAt(2); // inside1

            Assert.DoesNotThrow(() => hierarchyBuilder.Build(containers.OfType<IContainable>(), editables.OfType<IContainable>()));
        }
    }
}
