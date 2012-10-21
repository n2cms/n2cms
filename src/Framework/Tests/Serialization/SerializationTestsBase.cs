using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Persistence.Proxying;
using N2.Persistence.Serialization;
using N2.Tests.Fakes;
using N2.Tests.Serialization.Items;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Edit.FileSystem;

namespace N2.Tests.Serialization
{
	public abstract class SerializationTestsBase : ItemTestsBase
	{
		private delegate Url BuildUrl(ContentItem item);
		protected DefinitionManager definitions;
		protected ContentActivator activator;
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

			activator = new ContentActivator(new N2.Edit.Workflow.StateChanger(), notifier, new InterceptingProxyFactory());
			definitions = new DefinitionManager(
				new[] {new DefinitionProvider(new DefinitionBuilder(new DefinitionMap(), 
					finder, 
					new TransformerBase<IUniquelyNamed>[0],
					TestSupport.SetupEngineSection()))}, 
				new ITemplateProvider[0],
				activator, new StateChanger());
			definitions.Start();
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
										return url.ToUrl();
									}))
				.Repeat.Any();
			mocks.Replay(parser);

			persister = TestSupport.SetupFakePersister();
		}

		protected ItemXmlWriter CreateWriter()
		{
			return new ItemXmlWriter(definitions, parser, new FakeMemoryFileSystem());
		}

		protected Exporter CreateExporter()
		{
			return CreateExporter(new FakeMemoryFileSystem());
		}
		protected Exporter CreateExporter(IFileSystem fs)
		{
			return new Exporter(new ItemXmlWriter(definitions, parser, fs));
		}
		protected ItemXmlReader CreateReader()
		{
			return new ItemXmlReader(definitions, activator, persister.Repository);
		}
		protected Importer CreateImporter()
		{
			return CreateImporter(new FakeMemoryFileSystem());
		}
		protected Importer CreateImporter(IFileSystem fs)
		{
			return new Importer(persister, new ItemXmlReader(definitions, activator, persister.Repository), fs);
		}
	}
}
