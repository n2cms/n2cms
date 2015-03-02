using N2.Configuration;
using N2.Edit.Versioning;
using N2.Persistence;
using N2.Persistence.Serialization;
using N2.Persistence.Xml;
using N2.Web;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace N2.Tests.Persistence.Xml
{
	[TestFixture]
	public class XmlVersionRepositoryTests
	{
		XmlRepository<ContentVersion> repository;

		[SetUp]
		public void SetUp()
		{
			repository = new XmlRepository<ContentVersion>(new ThreadContext(), new XmlFileSystem(new ConfigurationManagerWrapper()));
		}

		[TearDown]
		public void TearDown()
		{
			repository.FileSystem.DeleteEntityDirectories();
		}

		[Test]
		public void Save()
		{
			var now = DateTime.Now;

			var version = new ContentVersion();
			version.Expired = now;
			version.FuturePublish = now;
			version.ItemCount = 3;
			version.Published = now;
			version.Saved = now;
			version.SavedBy = "Ben";
			version.State = ContentState.Published;
			version.Title = "Hello";
			version.VersionDataXml = "duh";
			version.VersionIndex = 3;

			using(repository)
			{
				repository.SaveOrUpdate(version);
			}
			var read = repository.Get(version.ID);

			read.Expired.ShouldBe(now);
			read.FuturePublish.ShouldBe(now);
			read.ItemCount.ShouldBe(3);
			read.Published.ShouldBe(now);
			read.Saved.ShouldBe(now);
			read.SavedBy.ShouldBe("Ben");
			read.State.ShouldBe(ContentState.Published);
			read.Title.ShouldBe("Hello");
			read.VersionDataXml.ShouldBe("duh");
			read.VersionIndex.ShouldBe(3);
		}

		[Test]
		public void Serialize_ContentItem()
		{
			var version = new ContentVersion();
			version.Title = "Hello";

			

			using (repository)
			{
				repository.SaveOrUpdate(version);
			}

			var read = repository.Get(version.ID);
			read.Title.ShouldBe(version.Title);
		}
	}
}
