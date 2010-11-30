using System;
using N2.Configuration;
using N2.Tests.Fakes;
using NUnit.Framework;
using N2.Definitions;
using N2.Persistence;
using N2.Persistence.Serialization;
using N2.Tests.Serialization.Items;
using N2.Web;
using Rhino.Mocks;
using N2.Persistence.Proxying;

namespace N2.Tests.Serialization
{
	public abstract class SerializationTestsBase : ItemTestsBase
	{
		private delegate string BuildUrl(ContentItem item);
		protected IDefinitionManager definitions;
        protected IUrlParser parser;
        protected IPersister persister;
		protected FakeTypeFinder finder;
		protected IItemNotifier notifier;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			finder = new FakeTypeFinder(typeof(XmlableItem).Assembly, typeof(XmlableItem), typeof(XmlableItem2));
			notifier = mocks.Stub<IItemNotifier>();
			mocks.Replay(notifier);

			definitions = new DefinitionManager(
				new DefinitionBuilder(
					finder, 
					new EngineSection(), 
					new FakeEditUrlManager()), 
				new N2.Edit.Workflow.StateChanger(), 
				notifier, 
				new InterceptingProxyFactory());

			parser = mocks.StrictMock<IUrlParser>();
			Expect.On(parser)
				.Call(parser.BuildUrl(null))
				.IgnoreArguments()
				.Do(new BuildUrl(delegate(ContentItem itemToBuild)
									{
										string url = "/" + itemToBuild.Name + ".aspx";
										foreach (ContentItem parent in Find.EnumerateParents(itemToBuild, null))
										{
											if (parent.Parent != null)
												url = "/" + parent.Name + url;
										}
										return url;
									}))
				.Repeat.Any();
			mocks.Replay(parser);

			persister = mocks.StrictMock<IPersister>();
			persister.Save(null);
			LastCall.IgnoreArguments().Repeat.Any();
			mocks.Replay(persister);
		}

		protected ItemXmlWriter CreateWriter()
		{
			return new ItemXmlWriter(definitions, parser);
		}
		protected Exporter CreateExporter()
		{
			return new Exporter(new ItemXmlWriter(definitions, parser));
		}
		protected ItemXmlReader CreateReader()
		{
			return new ItemXmlReader(definitions);
		}
		protected Importer CreateImporter()
		{
			return new Importer(persister, new ItemXmlReader(definitions));
		}
	}
}
