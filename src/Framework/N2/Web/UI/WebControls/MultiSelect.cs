using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
  public class MultiSelect : ListBox
  {
    public MultiSelect()
    {
      //SelectedList = 4;
    }

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
      SelectionMode = ListSelectionMode.Multiple;
      RegisterClientScript();
    }

    public int SearchTreshold { get; set; }

    public string ScriptUrl
    {
        get { return (string)(ViewState["ScriptUrl"] ?? "{ManagementUrl}/Resources/chosen_v1.1.0/chosen.jquery.min.js"); }
      set { ViewState["ScriptUrl"] = value; }
    }

    public string StyleSheetUrl
    {
        get { return (string)(ViewState["StyleSheetUrl"] ?? "{ManagementUrl}/Resources/chosen_v1.1.0/chosen.min.css"); }
      set { ViewState["StyleSheetUrl"] = value; }
    }

    private void RegisterClientScript()
    {
      Page.JQuery();
      Page.JQueryUi();
      Page.JavaScript(ScriptUrl);
      Page.StyleSheet(StyleSheetUrl);

      var options = new List<string>
                      {
                        string.Format("disable_search_threshold: {0}", SearchTreshold)
                      };
      var optionsString = string.Join(",", options.ToArray());

      Page.JavaScript(string.Format("$('#{0}').chosen({{{1}}});", ClientID, optionsString), ScriptPosition.Bottom, ScriptOptions.DocumentReady | ScriptOptions.ScriptTags);
    }


  }

}
