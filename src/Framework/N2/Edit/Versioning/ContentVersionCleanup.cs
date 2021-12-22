using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Plugin;
using N2.Persistence;
using N2.Engine;

namespace N2.Edit.Versioning
{
    //[Service]
    //Not in use - versions are deleted even if item deletion fails
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
