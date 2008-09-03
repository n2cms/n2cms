using System.Web.UI;

namespace N2.Templates.WebControls
{
    public class BookmarkList : Control
    {
        public bool ShowText
        {
            get { return (bool)(ViewState["ShowText"] ?? false); }
            set { ViewState["ShowText"] = value; }
        }

        public string BookmarkUrl
        {
            get { return (string)(ViewState["BookmarkUrl"] ?? string.Empty); }
            set { ViewState["BookmarkUrl"] = value; }
        }

        public string BookmarkText
        {
            get { return (string)(ViewState["BookmarkText"] ?? string.Empty); }
            set { ViewState["BookmarkText"] = value; }
        }

        public string ImageFolder
        {
            get { return (string)(ViewState["ImageFolder"] ?? string.Empty); }
            set { ViewState["ImageFolder"] = value; }
        }

		

    }
}