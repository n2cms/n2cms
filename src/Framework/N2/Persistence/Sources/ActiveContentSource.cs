using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Sources
{
    [ContentSource]
    public class ActiveContentSource : SourceBase<IActiveContent>
    {
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
            Active(item).Save();
        }

        public override void Delete(ContentItem item)
        {
            Active(item).Delete();
        }

        public override ContentItem Move(ContentItem source, ContentItem destination)
        {
            Active(source).MoveTo(destination);
            return source;
        }

        public override ContentItem Copy(ContentItem source, ContentItem destination)
        {
            return Active(source).CopyTo(destination);
        }

        private IActiveContent Active(ContentItem item)
        {
            return (IActiveContent)item;
        }
    }
}
