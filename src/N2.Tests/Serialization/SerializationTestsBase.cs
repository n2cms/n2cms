using System;
using NUnit.Framework;
using N2.Definitions;
using N2.Persistence;
using N2.Serialization;
using N2.Tests.Serialization.Items;
using N2.Web;
using Rhino.Mocks;

namespace N2.Tests.Serialization
{
	public class SerializationTestsBase : ItemTestsBase
	{
		private delegate string BuildUrl(ContentItem item);
		private delegate ContentItem CreateInstance(Type t, ContentItem parent);
		private IDefinitionManager definitions;
		private IUrlParser parser;
		private IPersister persister;
			
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			definitions = mocks.StrictMock<IDefinitionManager>();
			Expect.On(definitions)
				.Call(definitions.GetDefinition(typeof(XmlableItem)))
				.Return(new ItemDefinition(typeof(XmlableItem)))
				.Repeat.Any();
			Expect.On(definitions)
				.Call(definitions.GetDefinitions())
				.Return(new ItemDefinition[] {new ItemDefinition(typeof(XmlableItem))})
				.Repeat.Any();
			Expect.On(definitions)
				.Call(definitions.CreateInstance(typeof (XmlableItem), null))
				.Do(new CreateInstance(delegate (Type itemType, ContentItem parent)
										{
											ContentItem item = Activator.CreateInstance(itemType) as ContentItem;
											item.Parent = parent;
											return item;
										}))
				.Repeat.Any();
			mocks.Replay(definitions);
			
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
