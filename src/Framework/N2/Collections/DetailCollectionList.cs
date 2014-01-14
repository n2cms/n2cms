using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
    internal interface IEncolsedComponent
    {
        ContentItem EnclosingItem { get; set; }
    }

    public class DetailCollectionList : ContentList<DetailCollection>, IEncolsedComponent
    {
        ContentItem IEncolsedComponent.EnclosingItem { get; set; }

        public override DetailCollection FindNamed(string name)
        {
            return base.FindNamed(name)
                ?? new DetailCollection(((IEncolsedComponent)this).EnclosingItem, name) { Temporary = true };
        }
    }
}
