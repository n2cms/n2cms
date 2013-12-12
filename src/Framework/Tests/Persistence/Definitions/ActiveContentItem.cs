using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;

namespace N2.Tests.Persistence.Definitions
{
    public class ActiveContentItem : ContentItem, IActiveContent
    {
        public List<string> Actions = new List<string>();

        public void Save()
        {
            Actions.Add("Save");
        }

        public void Delete()
        {
            Actions.Add("Delete");
        }

        public void MoveTo(ContentItem destination)
        {
            Actions.Add("MoveTo " + destination);
        }

        public ContentItem CopyTo(ContentItem destination)
        {
            Actions.Add("CopyTo " + destination);
            return this.Clone(true);
        }
    }
}
