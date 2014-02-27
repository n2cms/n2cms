using N2.Definitions;
using N2.Edit;
using N2.Edit.Wizard.Items;
using N2.Engine;
using N2.Security.Items;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management
{
    [Adapts(typeof(ManagementItem))]
    public class ManagementItemsNodeAdapter : NodeAdapter
    {
        public override string GetPreviewUrl(ContentItem item, bool allowDraft)
        {
            if (item is UserList)
                return "{Account.Users.PageUrl}".ResolveUrlTokens();
            if (item is User)
                return "{Account.Users.Edit.PageUrl}".ToUrl().ResolveTokens().AppendQuery("user", item.Name);

            if (item is Wonderland)
                return "{ManagementUrl}/Content/Wizard/Default.aspx".ToUrl().ResolveTokens().AppendSelection(Engine.UrlParser.StartPage);

            return base.GetPreviewUrl(item, allowDraft);
        }
    }

    public abstract class ManagementItem : ContentItem, ISystemNode
    {
    }
}
