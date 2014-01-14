using System.Web.UI;
using N2.Plugin;
using N2.Security;

namespace N2.Edit
{
    public interface IControlPanelPlugin : IPlugin, ISecurable, IPermittable
    {
        Control AddTo(Control container, PluginContext context);
    }
}
