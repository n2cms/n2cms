using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Web.UI.WebControls;
using N2.Engine;
using N2.Web;

namespace N2.Edit.Web.UI.Controls
{
    public class ToolbarPluginDisplay : Control
    {
        public ToolbarArea Area { get; set; }
        public IEngine Engine { get { return N2.Context.Current; } }
        
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            var start = Engine.Resolve<IUrlParser>().StartPage;
            var root = Engine.Persister.Get(Engine.Resolve<IHost>().CurrentSite.RootItemID);
            foreach (ToolbarPluginAttribute plugin in Engine.EditManager.GetPlugins<ToolbarPluginAttribute>(Engine.Resolve<IWebContext>().User))
            {
                if ((plugin.Area & Area) != Area)
                    continue;

                HtmlGenericControl item = new HtmlGenericControl("div");
                item.Attributes["id"] = plugin.Name;
                item.Attributes["class"] = "item";
                Controls.Add(item);

                HtmlGenericControl command = new HtmlGenericControl("div");
                command.Attributes["class"] = "command";
                item.Controls.Add(command);

                if (plugin.OptionProvider != null)
                {
                    var optionProvider = Engine.Resolve(plugin.OptionProvider) as IProvider<ToolbarOption>;
                    var options = optionProvider.GetAll().ToList();

                    if (options.Count > 0)
                    {
                        OptionsMenu menu = new OptionsMenu();
                        menu.ID = ID + "_" + plugin.Name;
                        command.Controls.Add(menu);

                        AddPlugin(start, root, plugin, menu);

                        foreach (var option in options)
                        {
                            option.AddTo(menu);
                        }
                        continue;
                    }
                }
                
                AddPlugin(start, root, plugin, command);
            }
        }

        private void AddPlugin(ContentItem start, ContentItem root, ToolbarPluginAttribute plugin, Control command)
        {
            plugin.AddTo(command, new PluginContext(Page.GetSelection(), start, root,
                                                    ControlPanelState.Visible,
                                                    Engine, new HttpContextWrapper(Context)));
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class='" + Area.ToString().ToLower() + " toolbox'>");
            base.Render(writer);
            writer.Write("</div>");
        }
    }
}
