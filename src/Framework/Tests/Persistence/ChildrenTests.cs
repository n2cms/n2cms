using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Tests.Persistence.Definitions;
using N2.Collections;
using NUnit.Framework;
using Shouldly;
using N2.Security;

namespace N2.Tests.Persistence
{
	[TestFixture]
	public class ChildrenTests : PersistenceAwareBase
	{
		private N2.Persistence.IPersister persister;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			CreateDatabaseSchema();

			persister = engine.Persister;
		}

		[Test]
		public void Save_ChildState()
		{
			var item = new PersistableItem1();
			persister.Save(item);
			item.ChildState.ShouldBe(CollectionState.IsEmpty);
		}

		[Test]
		public void Save_WithChildren_ChildState()
		{
			var parent = new PersistableItem1();
			persister.Save(parent);

			var child = new PersistableItem1 { Parent = parent };
			persister.Save(child);

			parent.ChildState.ShouldBe(CollectionState.ContainsVisiblePublicPages);
		}

		[Test]
		public void Save_WithChildren_DoubleSave()
		{
			var parent = new PersistableItem1();
			persister.Save(parent);

			var child = new PersistableItem1 { Parent = parent };
			persister.Save(child);

			parent.Children.Count.ShouldBe(1);

			persister.Save(parent);

			parent.Children.Count.ShouldBe(1);
		}

		[Test]
		public void Save_WithDescendants()
		{
			var parent = new PersistableItem1();
			persister.Save(parent);

			var child1 = new PersistableItem1 { Parent = parent };
			persister.Save(child1);

			var child1_1 = new PersistableItem1 { Parent = child1 };
			persister.Save(child1_1);

			var child2 = new PersistableItem1 { Parent = parent };
			persister.Save(child2);

			var child2_1 = new PersistableItem1 { Parent = child2 };
			persister.Save(child2_1);

			persister.Save(parent);

			Find.EnumerateChildren(parent).Count().ShouldBe(4);
		}

		[TestCase(10)]
		//[TestCase(100)]
		//[TestCase(1000)]
		//[TestCase(10000)]
		public void Save_ManyAssociations_InSession(int iterations)
		{
			for (int i = 0; i < 100; i++)
			{
				var parent = new PersistableItem1();
				persister.Save(parent);

				var child1 = new PersistableItem1 { Parent = parent };
				child1["parent"] = parent;
				persister.Save(child1);

				var child1_1 = new PersistableItem1 { Parent = child1 };
				child1_1["parent"] = child1_1;
				persister.Save(child1_1);

				var child2 = new PersistableItem1 { Parent = parent };
				child1_1["sibling"] = child1;
				persister.Save(child2);

				var child2_1 = new PersistableItem1 { Parent = child2 };
				child1_1["cousin"] = child1_1;
				persister.Save(child2_1);

				child1["child"] = child1_1;
				child1["sibling"] = child2;
				persister.Save(child1);

				child1_1["cousin"] = child2_1;
				persister.Save(child1_1);

				child2["child"] = child2_1;
				persister.Save(child2);

				Find.EnumerateChildren(parent).Count().ShouldBe(4);
			}
		}
	}
}
