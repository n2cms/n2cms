using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Definitions;
using N2.Details;
using N2.Persistence.Search;
using N2.Installation;

namespace N2.Management.Content.LinkTracker
{
    [PageDefinition(IconUrl = "{IconsUrl}/error_go.png",
        InstallerVisibility = InstallerHint.NeverRootOrStartPage,
        TemplateUrl = "{ManagementUrl}/Resources/RedirectHandler.ashx",
        AuthorizedRoles = new string[0])]
    [Indexable(IsIndexable = false)]
    [Throwable(AllowInTrash.No)]
    public class PermanentRedirect : ContentItem, IRedirect, ISystemNode
    {
        public PermanentRedirect()
        {
            Visible = false;
        }

        public string RedirectUrl
        {
            get { return GetDetail<string>("RedirectUrl", null); }
            set { SetDetail("RedirectUrl", value); }
        }

        [EditableLink]
        public ContentItem RedirectTo
        {
            get { return GetDetail<ContentItem>("RedirectTo", null); }
            set { SetDetail("RedirectTo", value); }
        }
    }
}
