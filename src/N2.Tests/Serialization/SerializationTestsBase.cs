using System;
using NUnit.Framework;
using N2.Definitions;
using N2.Persistence;
using N2.Serialization;
using N2.Tests.Serialization.Items;
using N2.Web;
using Rhino.Mocks;
using N2.Engine;
using System.Reflection;

namespace N2.Tests.Serialization
{
	public class SerializationTestsBase : ItemTestsBase
	{
		private delegate string BuildUrl(ContentItem item);
		private delegate ContentItem CreateInstance(Type t, ContentItem parent);
		protected IDefinitionManager definitions;
        protected IUrlParser parser;
        protected IPersister persister;
		
        

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

            var finder = mocks.StrictMock<ITypeFinder>();
            Expect.Call(finder.GetAssemblies()).Return(new Assembly[] { typeof(XmlableItem).Assembly }); 
            Expect.Call(finder.Find(typeof(ContentItem))).Return(new Type[] { typeof(XmlableItem) });
            mocks.Replay(finder);
            var notifier = mocks.Stub<IItemNotifier>();
            mocks.Replay(notifier);

            definitions = new DefinitionManager(new DefinitionBuilder(finder), notifier);
			
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
