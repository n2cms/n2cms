using System;
using System.Collections.Generic;
using N2.Web.UI;

namespace N2.Addons.Tagging.UI
{
    public partial class Tag : ContentPage<Items.Tag>
    {
        protected IList<ContentItem> TaggedItems { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TaggedItems = Find.Items
                .Where.Detail().Eq(CurrentPage)
                .Select();
        }
    }
}
