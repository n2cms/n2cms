using System.Web.UI;
using N2.Plugin;

namespace N2.Edit
{
	public interface IControlPanelPlugin : IPlugin
	{
		Control AddTo(Control container, PluginContext context);
	}
}