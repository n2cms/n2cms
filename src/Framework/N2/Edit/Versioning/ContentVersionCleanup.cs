using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Plugin;
using N2.Persistence;

namespace N2.Edit.Versioning
{
    public class ContentVersionCleanup : IAutoStart
    {
        private IItemNotifier notifier;
        private ContentVersionRepository repository;

        public ContentVersionCleanup(IItemNotifier notifier, ContentVersionRepository repository)
        {
            this.notifier = notifier;
            this.repository = repository;


        }

        public void Start()
        {
            notifier.ItemDeleting += notifier_ItemDeleting;
        }

        void notifier_ItemDeleting(object sender, ItemEventArgs e)
        {
            repository.DeleteVersionsOf(e.AffectedItem);
        }

        public void Stop()
        {
            notifier.ItemDeleting -= notifier_ItemDeleting;
        }
    }
}
