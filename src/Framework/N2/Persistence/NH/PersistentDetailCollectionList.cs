using N2.Collections;
using N2.Details;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH
{
    [DebuggerDisplay("PersistentDetailCollectionList, Count = {Count}")]
    public class PersistentDetailCollectionList : PersistentContentList<DetailCollection>, IEncolsedComponent
    {
        public PersistentDetailCollectionList(NHibernate.Engine.ISessionImplementor session, IList<DetailCollection> list)
            : base(session, list)
        {
        }

        public PersistentDetailCollectionList(NHibernate.Engine.ISessionImplementor session)
            : base(session)
        {
        }

        ContentItem IEncolsedComponent.EnclosingItem { get; set; }

        public override DetailCollection FindNamed(string name)
        {
            return base.FindNamed(name)
                ?? new DetailCollection(((IEncolsedComponent)this).EnclosingItem, name) { Temporary = true };
        }
    }
}
