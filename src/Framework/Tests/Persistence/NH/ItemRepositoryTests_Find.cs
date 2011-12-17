using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using N2.Details;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Persistence.NH
{
	[TestFixture, Category("Integration")]
	public class ItemRepositoryTests_Finds : DatabasePreparingBase
	{
		ContentItemRepository repository;
		new ISessionProvider sessionProvider;
		private PersistableItem1 root;
		private PersistableItem1 child1;
		private PersistableItem1 grandchild1;
		private PersistableItem1 child2;
		private PersistablePart1 part1;
		private PersistablePart1 part2;
		private ContentItem[] all;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			CreateDatabaseSchema();

			sessionProvider = engine.Resolve<ISessionProvider>();
			repository = new ContentItemRepository(sessionProvider);

			all = new ContentItem []
			{
				root = CreateOneItem<Definitions.PersistableItem1>(0, "page", null),
				child1 = CreateOneItem<Definitions.PersistableItem1>(0, "page1", root),
				grandchild1 = CreateOneItem<Definitions.PersistableItem1>(0, "page1_1", child1),
				part1 = CreateOneItem<Definitions.PersistablePart1>(0, "part1", child1),
				part2 = CreateOneItem<Definitions.PersistablePart1>(0, "part1", child1),
				child2 = CreateOneItem<Definitions.PersistableItem1>(0, "page2", root),
			};
			part1.ZoneName = "Left";
			part2.ZoneName = "RecursiveLeft";

			repository.SaveOrUpdate(root);
		}

		[Test]
		public void Find_ByID_Equals()
		{
			repository.Find(Parameter.Equal("ID", grandchild1.ID)).Single().ShouldBe(grandchild1);
		}

		[Test]
		public void Find_ByID_NotEquals()
		{
			repository.Find(Parameter.NotEqual("ID", grandchild1.ID)).Count().ShouldBe(all.Length - 1);
		}

		[Test]
		public void Find_ByID_GreaterThan()
		{
			repository.Find(Parameter.GreaterThan("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID > grandchild1.ID));
		}

		[Test]
		public void Find_ByID_GreaterThanOrEqualTo()
		{
			repository.Find(Parameter.GreaterOrEqual("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID >= grandchild1.ID));
		}

		[Test]
		public void Find_ByID_LessThan()
		{
			repository.Find(Parameter.LessThan("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID < grandchild1.ID));
		}

		[Test]
		public void Find_ByID_LessThanOrEqualTo()
		{
			repository.Find(Parameter.LessOrEqual("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID <= grandchild1.ID));
		}

		[Test]
		public void Find_ByID_NotEqualTo()
		{
			repository.Find(Parameter.NotEqual("ID", grandchild1.ID)).SequenceEqual(all.Where(i => i.ID != grandchild1.ID));
		}

		[Test]
		public void Find_Zone_IsNull()
		{
			repository.Find(Parameter.IsNull("ZoneName")).SequenceEqual(all.Where(i => i.ZoneName == null));
		}

		[Test]
		public void Find_Zone_IsNotNull()
		{
			repository.Find(Parameter.IsNull("ZoneName")).SequenceEqual(all.Where(i => i.ZoneName != null));
		}

		[Test]
		public void Find_Zone_IsLike()
		{
			repository.Find(Parameter.StartsWith("ZoneName", "Recursive")).Single().ShouldBe(part2);
		}

		[Test]
		public void Find_Zone_IsNotLike()
		{
			repository.Find(Parameter.NotLike("ZoneName", "Recursive%")).Single().ShouldBe(part1);
		}

		[Test]
		public void Find_Or()
		{
			repository.Find(Parameter.Equal("ID", grandchild1.ID) | Parameter.Equal("ID", child1.ID)).Count().ShouldBe(2);
		}

		[Test]
		public void Find_And()
		{
			repository.Find(Parameter.Equal("ID", grandchild1.ID) & Parameter.Equal("Name", grandchild1.Name)).Single().ShouldBe(grandchild1);
		}

		[Test]
		public void Find_AndOr()
		{
			var results = repository.Find(
				(Parameter.Equal("Parent", root) & Parameter.StartsWith("Name", child1.Name))
				| (Parameter.Equal("Parent", root) & Parameter.StartsWith("Name", child2.Name))
				);

			results.Count().ShouldBe(2);
			results.ShouldNotContain(grandchild1);
		}

		[Test]
		public void Find_Skip()
		{
			var results = repository.Find(new ParameterCollection().Skip(2));

			results.Count().ShouldBe(all.Length - 2);
		}

		[Test]
		public void Find_Take()
		{
			var results = repository.Find(new ParameterCollection().Take(3));

			results.Count().ShouldBe(3);
		}

		[Test]
		public void Find_OrderBy_DESC()
		{
			var results = repository.Find(new ParameterCollection().OrderBy("ID"));

			results.First().ID.ShouldBe(all.Select(i => i.ID).Min());
			results.Last().ID.ShouldBe(all.Select(i => i.ID).Max());
		}

		[Test]
		public void Find_OrderBy()
		{
			var results = repository.Find(new ParameterCollection().OrderBy("ID Desc"));

			results.First().ID.ShouldBe(all.Select(i => i.ID).Max());
			results.Last().ID.ShouldBe(all.Select(i => i.ID).Min());
		}
	}
}
