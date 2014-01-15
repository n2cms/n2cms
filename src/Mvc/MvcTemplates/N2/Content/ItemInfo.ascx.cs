
using N2.Edit.Web;
namespace N2.Edit
{
    public partial class ItemInfo : EditUserControl
    {
        protected Definitions.ItemDefinition CurrentDefinition
        {
            get { return N2.Context.Definitions.GetDefinition(CurrentItem); }
        }

        public ContentItem currentItem;
        public ContentItem CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; }
        }

        protected string GetStateText(ContentItem item)
        {
            string fallback = item.State.ToString();
            return GetLocalResourceString(fallback, fallback);
        }
    }
}
