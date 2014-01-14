using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace N2.Edit.Web.UI.Controls
{
    public class CancelLink : HyperLink
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            CssClass += " cancel command";
            if (string.IsNullOrEmpty(NavigateUrl))
            {
                var page = Page as EditPage;
                if (page != null)
                {
                    Engine = page.Engine;
                    Selection = page.Selection;
                }
                NavigateUrl = CancelUrl();
            }
        }

        protected virtual string CancelUrl()
        {
            if (!string.IsNullOrEmpty(Page.Request["returnUrl"]))
                return Page.Request["returnUrl"];
            var item = Selection.SelectedItem.VersionOf.HasValue 
                ? Selection.SelectedItem.VersionOf.Value 
                : Selection.SelectedItem;
            return Engine.GetContentAdapter<NodeAdapter>(item).GetPreviewUrl(item);
        }

        Engine.IEngine engine;
        public Engine.IEngine Engine
        {
            get { return engine ?? (engine = N2.Context.Current); }
            set { engine = value; }
        }

        SelectionUtility selection;
        public SelectionUtility Selection
        {
            get { return selection ?? (selection = new SelectionUtility(this, Engine)); }
            set { selection = value; }
        }
    }
}
