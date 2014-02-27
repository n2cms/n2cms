using N2.Details;
using N2.Web.UI.WebControls;

namespace N2.Templates.Items
{
    [WithEditableTitle("Title", 10)]
    public abstract class SidebarItem : AbstractItem
    {
        [DisplayableHeading(4)]
        public override string Title
        {
            get { return base.Title; } 
            set { base.Title = value; }
        }
    }
}
