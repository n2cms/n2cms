using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using N2.Collections;

namespace N2.Tests.Details.Models
{

    public class DecoratedItem2 : DecoratedItem
    {
        [EditableItem(DefaultChildName = "OtherChildName")]
        public virtual OtherItem EditableItemWithDefaultChildName { get; set; }

        [EditableItem(DefaultChildZoneName = "OtherZone")]
        public virtual OtherItem EditableItemWithDefaultChildZoneName { get; set; }
    }

    [WithEditableTitle]
    [WithEditableName]
    public class DecoratedItem : ContentItem
    {
        [EditableChildren("Children", "Children", 100)]
        public virtual ItemList EditableChildren { get; set; }

        [EditableChildren]
        public virtual IList<BaseItem> GenericChildren { get; set; }

        [EditableChildren(AllowedTemplateKeys = new string[] { "Index", "Nonedex" })]
        public virtual IList<BaseItem> TemplateChildren { get; set; }

        [EditableItem]
        public virtual OtherItem TheItem { get; set; }

		[Obsolete]
        public override ItemList GetChildren(string childZoneName)
        {
            return base.GetChildren(new ZoneFilter(childZoneName));
        }
    }

    [WithEditableTitle]
    [WithEditableName]
    public class OtherItem : ContentItem
    {
    }

    public class BaseItem : ContentItem
    {
    }

    public class SuperficialItem : BaseItem
    {
    }
}
