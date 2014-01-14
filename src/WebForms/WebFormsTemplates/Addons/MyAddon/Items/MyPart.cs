using System;
using N2.Details;
using N2.Integrity;

namespace N2.Addons.MyAddon.Items
{
    /// <summary>
    /// To spice things up there is a scheduled action that finds all MyParts and 
    /// updates their properties on a regular basis. Far-fetched? Yes. 
    /// </summary>
    [Definition("My Part")]
    [AllowedZones(AllowedZones.All)]
    [RestrictParents(typeof(MyPage))]
    public class MyPart : ContentItem
    {
        public override bool IsPage
        {
            get { return false; }
        }


        [Editables.EditableReset("Times Visited", 100)]
        public virtual int TimesVisited
        {
            get { return GetDetail("TimesVisited", 0); }
            set { SetDetail("TimesVisited", value, 0); }
        }


        [EditableTextBox("Last Visited", 100)]
        public virtual DateTime? LastVisited
        {
            get { return (DateTime?)GetDetail("LastVisited"); }
            set { SetDetail("LastVisited", value); }
        }


        public override string TemplateUrl
        {
            get { return "~/Addons/MyAddon/UI/MyPart.ascx"; }
        }
    }
}
