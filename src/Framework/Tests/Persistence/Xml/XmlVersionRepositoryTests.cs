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
		private ItemXmlWriter writer;
		private ItemXmlReader reader;
		private N2.Definitions.IDefinitionManager definitions;
		private ItemNotifier notifier;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			definitions = TestSupport.SetupDefinitions(typeof(Definitions.PersistableItem), typeof(Definitions.PersistablePart));
			writer = new ItemXmlWriter(
						definitions,
						TestSupport.SetupFileSystem());
			reader = new ItemXmlReader(
						definitions,
						TestSupport.SetupContentActivator());
		}

		[SetUp]
		public void SetUp()
		{
			notifier = new ItemNotifier();
			repository = new XmlRepository<ContentVersion>(definitions, new ThreadContext(), new ConfigurationManagerWrapper());
		}

		[TearDown]
		public void TearDown()
		{
			foreach (var file in Directory.GetFiles(repository.DataDirectoryPhysical, "*.xml"))
				File.Delete(file);
		}

		[Test]
		public void Save()
		{
			var version = new ContentVersion();
			version.Title = "Hello";

			using(repository)
			{
				repository.SaveOrUpdate(version);
			}

			var read = repository.Get(version.ID);
			read.Title.ShouldBe(version.Title);
		}
	}
}
