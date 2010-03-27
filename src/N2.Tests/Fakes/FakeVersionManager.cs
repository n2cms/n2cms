using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using N2.Edit.Workflow;

namespace N2.Tests.Fakes
{
    public class FakeVersionManager : IVersionManager
    {
        FakeRepository<ContentItem> itemRepository;
        VersionManager original;

        public FakeVersionManager(FakeRepository<ContentItem> itemRepository, StateChanger stateChanger)
		{
            this.itemRepository = itemRepository;
            original = new VersionManager(itemRepository, null, stateChanger);
		}
        #region IVersionManager Members

        public ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem)
        {
            return original.ReplaceVersion(currentItem, replacementItem);
        }

        public ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem, bool storeCurrentVersion)
        {
            return original.ReplaceVersion(currentItem, replacementItem, storeCurrentVersion);
        }

        public ContentItem SaveVersion(ContentItem item)
        {
            return original.SaveVersion(item);
        }

        public IList<ContentItem> GetVersionsOf(ContentItem publishedItem)
        {
            return itemRepository.database.Values.Where(i => i.VersionOf == publishedItem || i == publishedItem).OrderByDescending(i => i.VersionIndex).ToList();
        }

        public IList<ContentItem> GetVersionsOf(ContentItem publishedItem, int count)
        {
            return GetVersionsOf(publishedItem).Take(count).ToList();
        }

        public void TrimVersionCountTo(ContentItem publishedItem, int maximumNumberOfVersions)
        {
        }

		public bool IsVersionable(ContentItem item)
		{
			return original.IsVersionable(item);
		}

		#endregion
	}
}
