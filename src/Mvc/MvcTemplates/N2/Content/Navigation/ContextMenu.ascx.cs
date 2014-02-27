using System;
using System.Web;
using N2.Edit.Web;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit.Navigation
{
    public partial class ContextMenu : EditUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            var start = Engine.Resolve<IUrlParser>().StartPage;
            var root = Engine.Persister.Repository.Get(Engine.Resolve<IHost>().CurrentSite.RootItemID);
            foreach (NavigationPluginAttribute a in Engine.EditManager.GetPlugins<NavigationPluginAttribute>(Page.User))
            {
                a.AddTo(plhMenuItems, new PluginContext(Selection, start, root, ControlPanelState.Visible,
                                                        Engine, new HttpContextWrapper(Context)));
            }
            base.OnInit(e);
        }
    }
}
