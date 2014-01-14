using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Versioning;
using N2.Web;
using N2.Engine;

namespace N2.Persistence.Sources
{
    [ContentSource]
    public class VersionSource : SourceBase<IActiveContent>
    {
        private ContentVersionRepository repository;

        public VersionSource(IEngine engine)
        {
            this.Engine = engine;
        }

        public ContentVersionRepository Repository
        {
            get { return repository ?? (repository = Engine.Resolve<ContentVersionRepository>()); }
            set { repository = value; }
        }

        public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
        {
            return previousChildren;
        }

        public override ContentItem Get(object id)
        {
            return null;
        }

        public override void Save(ContentItem item)
        {
            if (!item.VersionOf.HasValue)
                return;
            Repository.Save(item);
        }

        public override void Delete(ContentItem item)
        {
            if (!item.VersionOf.HasValue)
                return;
            Repository.Delete(item);
        }

        public override ContentItem Move(ContentItem source, ContentItem destination)
        {
            throw new NotSupportedException("Moving versions not supported");
        }

        public override ContentItem Copy(ContentItem source, ContentItem destination)
        {
            throw new NotSupportedException("Copying versions not supported");
        }

        public override bool IsProvidedBy(N2.ContentItem item)
        {
            return item.VersionOf.HasValue;
        }
    }
}
