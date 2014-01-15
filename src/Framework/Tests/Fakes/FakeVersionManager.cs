using System.Collections.Generic;
using System.Linq;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Edit.Versioning;
using N2.Web;
using System;

namespace N2.Tests.Fakes
{
    public class FakeVersionManager : VersionManager
    {
        FakeRepository<ContentItem> itemRepository;

        public FakeVersionManager(FakeContentItemRepository itemRepository, StateChanger stateChanger, params Type[] definitionTypes)
            : base(TestSupport.CreateVersionRepository(definitionTypes), itemRepository, stateChanger, new N2.Configuration.EditSection())
        {
            this.itemRepository = itemRepository;
        }

        #region IVersionManager Members

        public override void TrimVersionCountTo(ContentItem publishedItem, int maximumNumberOfVersions)
        {
        }

        #endregion
    }
}
