using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Web;
using N2.Persistence;

namespace N2.Edit.Versioning
{
    [Service]
    public class ContentVersionRepository
    {
        private IRepository<ContentVersion> repository;

        public ContentVersionRepository(IRepository<ContentVersion> repository)
        {
            this.repository = repository;
        }

        public IEnumerable<ContentVersion> GetDrafts()
        {
            yield break;
        }
    }
}
