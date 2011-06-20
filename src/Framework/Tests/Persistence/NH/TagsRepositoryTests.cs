using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence;
using N2.Tests.Persistence.Definitions;

namespace N2.Tests.Persistence
{
	[TestFixture]
	public class TagsRepositoryTests : DatabasePreparingBase
	{
		TagsRepository repository;
		ContentItem item;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			item = CreateOneItem<PersistableItem2>(0, "item", null);
			item.State = ContentState.Published;
			repository = new TagsRepository(engine);
		}

		[Test]
		public void SettingTag_MakesItGettable()
		{
			repository.SetTags(item, "Tags", new string[] { "Hello", "World" });
			var tags = repository.GetTags(item, "Tags");

			Assert.That(tags, Is.EquivalentTo(new string[] { "Hello", "World" }));
		}

		[Test]
		public void SavingTag_MakesTagsFindable()
		{
			repository.SaveTags(item, "Tags", new string[] { "Hello", "World" });

			var tags = repository.FindTags(item, "Tags");

			Assert.That(tags.Count(), Is.EqualTo(2));
			Assert.That(tags.Contains("Hello"));
			Assert.That(tags.Contains("World"));
		}

		[Test]
		public void SavingTag_MakesItemFindable()
		{
			repository.SaveTags(item, "Tags", new string[] { "Hello", "World" });

			var taggedItems = repository.FindTagged(item, "Tags", "Hello");

			Assert.That(taggedItems, Is.EquivalentTo(new [] { item }));
		}
	}
}
