using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Definitions;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class EditableExplorerTests : ItemTestsBase
	{
		EditableHierarchyBuilder hierarchyBuilder = new EditableHierarchyBuilder();
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

		//	- root
		//		- name
		//		- fieldset
		//			- title
		//			- fieldset
		//				- freetextarea
		[Test]
		public void CanBuildContainerAndEditableStructure()
		{
			Type itemType = typeof(Items.DefinitionTextPage);
			IList<IEditable> editables = explorer.Find<IEditable>(itemType);
			IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(itemType);
			IEditableContainer rootContainer = hierarchyBuilder.Build(containers, editables, "");

			List<IContainable> contained = rootContainer.GetContained(null);
			Assert.AreEqual(2, contained.Count);

			Assert.AreEqual(typeof(N2.Details.WithEditableNameAttribute), contained[0].GetType());
			
			IEditableContainer fieldSet = contained[1] as IEditableContainer;
			Assert.IsNotNull(fieldSet);
			Assert.AreEqual(typeof(N2.Web.UI.FieldSetContainerAttribute), fieldSet.GetType());

			List<IContainable> containedByFieldSet = fieldSet.GetContained(null);
			Assert.AreEqual(typeof(N2.Details.WithEditableTitleAttribute), containedByFieldSet[0].GetType());

			IEditableContainer innerFieldSet = containedByFieldSet[1] as IEditableContainer;
			Assert.IsNotNull(innerFieldSet);
			Assert.AreEqual(typeof(N2.Web.UI.FieldSetContainerAttribute), innerFieldSet.GetType());

			List<IContainable> containedByInnerFieldSet = innerFieldSet.GetContained(null);
			Assert.AreEqual(typeof(N2.Details.EditableFreeTextAreaAttribute), containedByInnerFieldSet[0].GetType());
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
			IEditableContainer rootContainer = hierarchyBuilder.Build(containers, editables, "");

			IContainable property0 = rootContainer.GetContained(null)[0];
			Assert.AreSame(property0, editables[0]);

			IEditableContainer first = rootContainer.GetContained(null)[1] as IEditableContainer;
			Assert.AreSame(first, containers[1]);

			IContainable property1 = first.GetContained(null)[1];
			Assert.AreSame(property1, editables[1]);

			IEditableContainer inside1 = first.GetContained(null)[2] as IEditableContainer;
			Assert.AreEqual(inside1, containers[2]);

			IContainable property2 = inside1.GetContained(null)[0];
			Assert.AreSame(property2, editables[2]);

			IEditableContainer inside1_1 = (IEditableContainer)inside1.GetContained(null)[1];
			Assert.AreEqual(inside1_1, containers[4]);
		}

		[Test]
		public void ContainersAreNestedAndSorted()
		{
			Type itemType = typeof(Definitions.ItemWithNestedContainers);
			IList<IEditable> editables = new List<IEditable>();
			IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(itemType);
			IEditableContainer rootContainer = hierarchyBuilder.Build(containers, editables, "");

			IEditableContainer first = rootContainer.GetContained(null)[0] as IEditableContainer;
			Assert.IsNotNull(first);
			Assert.AreSame(first, containers[1]);
			Assert.AreEqual(3, first.GetContained(null).Count);
			
			IEditableContainer inside3 = first.GetContained(null)[0] as IEditableContainer;
			Assert.IsNotNull(inside3);
			Assert.AreSame(inside3, containers[0]);
			Assert.AreEqual(0, inside3.GetContained(null).Count);

			IEditableContainer inside1 = first.GetContained(null)[1] as IEditableContainer;
			Assert.IsNotNull(inside1);
			Assert.AreSame(inside1, containers[2]);
			Assert.AreEqual(1, inside1.GetContained(null).Count);

			IEditableContainer inside2 = first.GetContained(null)[2] as IEditableContainer;
			Assert.IsNotNull(inside2);
			Assert.AreSame(inside2, containers[3]);
			Assert.AreEqual(0, inside2.GetContained(null).Count);
		}

		[Test]
		public void InvalidContainerReference_IsIgnored()
		{
			Type itemType = typeof(N2.Tests.Definitions.Definitions.ItemWithNestedContainers);
			IList<IEditable> editables = new List<IEditable>();
			IList<IEditableContainer> containers = explorer.Find<IEditableContainer>(itemType);
			containers.RemoveAt(2); // inside1

			Assert.DoesNotThrow(() => hierarchyBuilder.Build(containers, editables, ""));
		}
	}
}
