using N2.Configuration;
using N2.Persistence;
using N2.Persistence.Serialization;
using N2.Persistence.Xml;
using N2.Web;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Persistence.Xml
{
	public abstract class XmlRepositoryTestsBase : ItemTestsBase
	{
		protected XmlContentRepository repository;
		protected ItemXmlWriter writer;
		protected ItemXmlReader reader;
		protected N2.Definitions.IDefinitionManager definitions;
		protected ItemNotifier notifier;

		protected override T CreateOneItem<T>(int id, string name, ContentItem parent)
		{
			var item = base.CreateOneItem<T>(id, name, parent);
			repository.SaveOrUpdate(item);
			return item;
		}

		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
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
		public override void SetUp()
		{
			base.SetUp();
			notifier = new ItemNotifier();
			repository = new XmlContentRepository(definitions, new ThreadContext(), new XmlFileSystem(new ConfigurationManagerWrapper()), writer, reader, notifier);
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			repository.FileSystem.DeleteEntityDirectories();
		}

	}
}
