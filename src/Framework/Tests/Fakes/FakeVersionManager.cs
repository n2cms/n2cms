using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using N2.Edit.Workflow;

namespace N2.Tests.Fakes
{
	public class FakeVersionManager : VersionManager
    {
        FakeRepository<ContentItem> itemRepository;

        public FakeVersionManager(FakeRepository<ContentItem> itemRepository, StateChanger stateChanger)
			: base(itemRepository, null, stateChanger, new N2.Configuration.EditSection())
		{
            this.itemRepository = itemRepository;
		}

        #region IVersionManager Members

        public override IList<ContentItem> GetVersionsOf(ContentItem publishedItem)
        {
            return itemRepository.database.Values.Where(i => i.VersionOf == publishedItem || i == publishedItem).OrderByDescending(i => i.VersionIndex).ToList();
        }

        public override IList<ContentItem> GetVersionsOf(ContentItem publishedItem, int count)
        {
            return GetVersionsOf(publishedItem).Take(count).ToList();
        }

        public override void TrimVersionCountTo(ContentItem publishedItem, int maximumNumberOfVersions)
        {
        }

		#endregion
	}
}
