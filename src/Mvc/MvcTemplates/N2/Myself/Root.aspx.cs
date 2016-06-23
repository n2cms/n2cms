using System.Collections.Generic;
using System.Web.UI;
using N2.Edit;
using N2.Edit.Web;
using N2.Web;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Management.Myself
{
    [ToolbarPlugin("HOME", "home", "{ManagementUrl}/Myself/Root.aspx", ToolbarArea.Navigation, Targets.Preview, "{ManagementUrl}/Resources/icons/application_home.png", -50,
        Legacy = true)]
    public partial class Root : EditPage, IContentTemplate, IItemContainer
    {
        protected override void OnPreInit(System.EventArgs e)
        {
            base.OnPreInit(e);

            if (CurrentItem == null)
            {
                var root = Engine.Persister.Repository.Get(Engine.Resolve<IHost>().CurrentSite.RootItemID);
                Response.Redirect(root.Url);
            }
            else
            {
                var path = Engine.Resolve<RequestPathProvider>().ResolveUrl(Engine.RequestContext.Url);
                CurrentItem = path.CurrentItem;
                Engine.RequestContext.CurrentPath = path;
            }
        }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            cp.Visible = Engine.SecurityManager.IsAdmin(User);

            Title = CurrentItem["AlternativeTitle"] as string;
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);

            if (!ControlPanel.GetState(this).IsFlagSet(ControlPanelState.DragDrop))
            {
                HideIfEmpty(c1, Zone2.DataSource);
                HideIfEmpty(c2, Zone3.DataSource);
                HideIfEmpty(c3, Zone4.DataSource);
            }
        }

        private void HideIfEmpty(Control container, IList<ContentItem> items)
        {
            container.Visible = items != null && items.Count > 0;
        }

        #region IContentTemplate Members

        private ContentItem currentItem;

        public ContentItem CurrentItem
        {
            get { return currentItem ?? (currentItem = Engine.UrlParser.Parse(Request.RawUrl)); }
            set { currentItem = value; }
        }

        #endregion
    }
}
