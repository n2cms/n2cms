using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web.Parts;
using N2.Edit;
using N2.Configuration;
using N2.Edit.Versioning;
using N2.Persistence;
using N2.Web;
using N2.Persistence.Sources;
using N2.Security;
using System.Collections.Specialized;
using Shouldly;
using N2.Details;
using N2.Definitions;
using N2.Definitions.Static;

namespace N2.Tests.Web.Parts
{
    [TestFixture]
    public class CreateUrlProviderTests : ItemPersistenceMockingBase
    {
        private CreateUrlProvider creator;
        private NameValueCollection request;
        private Items.PageItem root;
        private N2.Definitions.IDefinitionManager definitions;
        private ContentVersionRepository versionRepository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var types = new[] { typeof(Items.PageItem), typeof(Items.DataItem) };
            versionRepository = TestSupport.CreateVersionRepository(types);
            var versions = TestSupport.SetupVersionManager(persister, versionRepository);
            //definitions = TestSupport.SetupDefinitions(types);

            ITemplateAggregator templates;
            ContentActivator activator;
            TestSupport.Setup(out definitions, out templates, out activator, types);
            creator = new CreateUrlProvider(
                persister,
                new EditUrlManager(null, new EditSection()),
                definitions,
                templates,
                activator,
                new Navigator(persister, TestSupport.SetupHost(), new VirtualNodeFactory(), TestSupport.SetupContentSource()),
                versions, 
                versionRepository);
            request = new NameValueCollection();

            root = CreateOneItem<Items.PageItem>(0, "root", null);
        }

        [Test]
        public void CreateUrlProvider_CratesItems_WithoutEditables_AndRedirectsTo_DraftVersion()
        {
            request["discriminator"] = "DataItem";
            request["returnUrl"] = "/back/to/here?edit=drag";
            request["below"] = root.Path;
            request["zone"] = "SomeZone";
            long initialCount = persister.Repository.Count();
            
            var response = creator.HandleRequest(request);
            
            response["dialog"].ShouldBe("no");
            response["redirect"].ShouldBe("/back/to/here?edit=drag&n2versionIndex=1");
            persister.Repository.Count().ShouldBe(initialCount);
            versionRepository.Repository.Count().ShouldBe(1);
			versionRepository.DeserializeVersion(versionRepository.GetVersion(root)).Children.Single().ShouldBeOfType<Items.DataItem>();
        }

        [Test]
        public void CreateUrlProvider_RedirectsTo_EditInterfaceUrl_WhenItemHasEditables()
        {
            definitions.GetDefinition(typeof(Items.DataItem)).Editables.Add(new EditableTextAttribute { Name = "Title" });

            request["discriminator"] = "DataItem";
            request["returnUrl"] = "/back/to/here";
            request["below"] = root.Path;
            long initialCount = persister.Repository.Count();

            var response = creator.HandleRequest(request);

            response["dialog"].ShouldBe("yes");
            response["redirect"].ShouldStartWith("/N2/Content/Edit.aspx");
            persister.Repository.Count().ShouldBe(initialCount);
        }
    }
}
