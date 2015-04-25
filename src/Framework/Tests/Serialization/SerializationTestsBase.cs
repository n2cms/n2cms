using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Persistence.Proxying;
using N2.Persistence.Serialization;
using N2.Persistence.Xml;
using N2.Tests.Fakes;
using N2.Tests.Serialization.Items;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Edit.FileSystem;
using System.Linq;

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
                activator, new StateChanger(), new DefinitionMap());
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

        protected abstract IItemXmlWriter CreateWriter();
        protected abstract Exporter CreateExporter();
        protected abstract Exporter CreateExporter(IFileSystem fs);
        protected abstract IItemXmlReader CreateReader();
        protected abstract Importer CreateImporter();
        protected abstract Importer CreateImporter(IFileSystem fs);

        protected XPathNavigator WriteToStreamAndNavigate(ContentItem item)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(WriteToStreamAndReturn(item));
            return xd.CreateNavigator();
        }

        protected string WriteToStreamAndReturn(ContentItem item)
        {
            IItemXmlWriter writer = CreateWriter();
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            XmlTextWriter xmlOutput = new XmlTextWriter(sw);
            xmlOutput.Formatting = Formatting.Indented;
            xmlOutput.Indentation = 1;
            xmlOutput.WriteStartDocument();
            xmlOutput.WriteStartElement("n2");

            writer.Write(item, ExportOptions.Default, xmlOutput);

            xmlOutput.WriteEndElement();
            xmlOutput.WriteEndDocument();
            return sb.ToString();
        }
    }

    public class XmlSerializationTestsBase : SerializationTestsBase
    {
        protected override IItemXmlWriter CreateWriter()
        {
            return (new ItemXmlWriter(definitions, new FakeMemoryFileSystem()));
        }

        protected override Exporter CreateExporter()
        {
            return CreateExporter(new FakeMemoryFileSystem());
        }

        protected override Exporter CreateExporter(IFileSystem fs)
        {
            return new Exporter(new ItemXmlWriter(definitions, fs));
        }

        protected override IItemXmlReader CreateReader()
        {
            return new ItemXmlReader(definitions, activator);
        }

        protected override Importer CreateImporter()
        {
            return CreateImporter(new FakeMemoryFileSystem());
        }

        protected override Importer CreateImporter(IFileSystem fs)
        {
            return new Importer(persister, new ItemXmlReader(definitions, activator), fs);
        }
    }

    public class HtmlSerializationTestsBase : SerializationTestsBase
    {
        protected override IItemXmlWriter CreateWriter()
        {
            return (new ItemHtmlWriter(definitions, new FakeMemoryFileSystem()));
        }

        protected override Exporter CreateExporter()
        {
            return CreateExporter(new FakeMemoryFileSystem());
        }
        protected override Exporter CreateExporter(IFileSystem fs)
        {
            return new Exporter(new ItemHtmlWriter(definitions,fs));
        }

        protected override IItemXmlReader CreateReader()
        {
            return new ItemHtmlReader(); //(definitions, activator, persister.Repository);
        }

        protected override Importer CreateImporter()
        {
            return CreateImporter(new FakeMemoryFileSystem());
        }

        protected override Importer CreateImporter(IFileSystem fs)
        {
            return new Importer(persister, new ItemHtmlReader(), fs);
        }
    }

}
