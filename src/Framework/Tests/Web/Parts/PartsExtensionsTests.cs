using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;
using N2.Web.Parts;
using N2.Tests.Web.Items;

namespace N2.Tests.Web.Parts
{
	[TestFixture]
	public class PartsExtensionsTests
	{
		[Test]
		public void LoadEmbeddedPart_CreatesEmptyPart()
		{
			var item = new CustomItem();
			var part = item.LoadEmbeddedPart<DataItem>("Hello");

			part.ShouldNotBe(null);
			part.ShouldBeTypeOf<DataItem>();
		}

		[Test]
		public void StoreEmbeddedPart_EmbedsPartProperties()
		{
			var item = new CustomItem();
			item.StoreEmbeddedPart("Hello", new DataItem { Title = "Hello World" });

			var part = item.LoadEmbeddedPart<DataItem>("Hello");

			part.Title.ShouldBe("Hello World");
		}

		[Test]
		public void StoreEmbeddedPart_EmbedsPartDetails()
		{
			var item = new CustomItem();
			var part = new DataItem();
			part["Hello"] = "World";
			item.StoreEmbeddedPart("Hello", part);

			var loadedPart = item.LoadEmbeddedPart<DataItem>("Hello");

			loadedPart["Hello"].ShouldBe("World");
		}

		[Test]
		public void StoreEmbeddedPart_OverwritesPreviousProperties()
		{
			var item = new CustomItem();
			item.StoreEmbeddedPart("Hello", new DataItem { Title = "Hello World" });
			item.StoreEmbeddedPart("Hello", new DataItem { Title = "Hello Multiverse!!" });

			var part = item.LoadEmbeddedPart<DataItem>("Hello");

			part.Title.ShouldBe("Hello Multiverse!!");
		}

		[Test]
		public void StoreEmbeddedPart_OverwritesPreviousDetails()
		{
			var item = new CustomItem();
			var part = new DataItem();
			part["Hello"] = "World";
			item.StoreEmbeddedPart("Hello", part);
			var part2 = new DataItem();
			part["Hello"] = "Multiverse!!";
			item.StoreEmbeddedPart("Hello", part);

			var loadedPart = item.LoadEmbeddedPart<DataItem>("Hello");

			loadedPart["Hello"].ShouldBe("Multiverse!!");
		}

		[Test]
		public void MultipleParts_CanBeStored()
		{
			var item = new CustomItem();
			item.StoreEmbeddedPart("Embedded1", new DataItem { Title = "First" });
			item.StoreEmbeddedPart("Embedded2", new DataItem { Title = "Second" });

			var part1 = item.LoadEmbeddedPart<DataItem>("Embedded1");
			var part2 = item.LoadEmbeddedPart<DataItem>("Embedded2");

			part1.Title.ShouldBe("First");
			part2.Title.ShouldBe("Second");
		}
	}
}
