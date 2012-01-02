using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web;
using Shouldly;
using N2.Persistence;

namespace N2.Raven.Tests
{
	[TestFixture]
	public class RavenRepositoryTests
	{
		private RavenConnectionProvider connectionProvider;
		private RavenContentRepository repository;

		[SetUp]
		public void SetUp()
		{
			connectionProvider = new RavenConnectionProvider(new RavenStoreFactory(new Configuration.DatabaseSection()) { RunInMemory = true }, new ThreadContext());
			repository = new RavenContentRepository(connectionProvider);
		}

		[Test]
		public void SaveOrUpdate_PropertiesArePersisted()
		{
			var item = new MyItem();
			item.Title = "Hello";
			item.Name = "world";
			item.ZoneName = "Left";
			repository.SaveOrUpdate(item);

			var storedItem = repository.Get(item.ID);
			storedItem.ShouldNotBe(null);
			storedItem.Title.ShouldBe(item.Title);
			storedItem.Name.ShouldBe(item.Name);
			storedItem.ZoneName.ShouldBe(item.ZoneName);
		}

		[Test]
		public void SaveOrUpdate_DetailsArePersisted()
		{
			var item = new MyItem();
			item["string"] = "a string";
			item["int"] = 123;
			item["double"] = 123.456;
			item["bool"] = true;
			repository.SaveOrUpdate(item);

			var storedItem = repository.Get(item.ID);
			storedItem.ShouldNotBe(null);
			storedItem["string"].ShouldBe(item["string"]);
			storedItem["int"].ShouldBe(item["int"]);
			storedItem["double"].ShouldBe(item["double"]);
			storedItem["bool"].ShouldBe(item["bool"]);
		}

		[Test]
		public void SaveOrUpdate_Children_AreRetrieved()
		{
			var parent = new MyItem { Title = "parent" };
			var child = new MyItem { Title = "child" };
			child.AddTo(parent);
			repository.SaveOrUpdate(parent);
			repository.SaveOrUpdate(child);

			var storedItem = repository.Get(parent.ID);
			storedItem.Children.Single().Title.ShouldBe(child.Title);
		}

		[Test]
		public void SaveOrUpdate_Children_AreRetrieved_ByChildReference()
		{
			var parent = new MyItem { Title = "parent" };
			repository.SaveOrUpdate(parent);

			var child = new MyItem { Title = "child" };
			child.AddTo(parent);
			repository.SaveOrUpdate(child);

			var storedItem = repository.Get(parent.ID);
			storedItem.Children.Single().Title.ShouldBe(child.Title);
		}

		[Test]
		public void Find_ByParameter()
		{
			repository.SaveOrUpdate(new MyItem { Name = "Hello" });
			repository.SaveOrUpdate(new MyItem { Name = "Hello2" });

			repository.Find(Parameter.Equal("Name", "Hello"))
				.Single().Name.ShouldBe("Hello");
		}

		[Test]
		public void Find_ByParameter_And_Parameter()
		{
			repository.SaveOrUpdate(new MyItem { Title = "Hello", Name = "Hello" });
			repository.SaveOrUpdate(new MyItem { Title = "Hello", Name = "World" });

			repository.Find(Parameter.Equal("Title", "Hello") & Parameter.Equal("Name", "Hello"))
				.Single().Name.ShouldBe("Hello");
		}

		[Test]
		public void Find_ByParameter_And_NotParameter()
		{
			repository.SaveOrUpdate(new MyItem { Title = "Hello", Name = "Hello" });
			repository.SaveOrUpdate(new MyItem { Title = "Hello", Name = "World" });

			repository.Find(Parameter.Equal("Title", "Hello") & Parameter.NotEqual("Name", "Hello"))
				.Single().Name.ShouldBe("World");
		}
	}
}
