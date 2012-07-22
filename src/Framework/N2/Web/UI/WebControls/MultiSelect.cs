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
      EnableFilter = false;
      SelectedList = 4;
    }

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
      SelectionMode = ListSelectionMode.Multiple;
      RegisterClientScript();
    }

    public bool EnableFilter { get; set; }

    public int SelectedList { get; set; }

    public string ScriptUrl
    {
      get { return (string)(ViewState["ScriptUrl"] ?? "{ManagementUrl}/Resources/Js/jquery.multiselect.js"); }
      set { ViewState["ScriptUrl"] = value; }
    }

    public string StyleSheetUrl
    {
      get { return (string)(ViewState["StyleSheetUrl"] ?? "{ManagementUrl}/Resources/Css/jquery.multiselect.css"); }
      set { ViewState["StyleSheetUrl"] = value; }
    }

    public string FilterScriptUrl
    {
      get { return (string)(ViewState["ScriptUrl"] ?? "{ManagementUrl}/Resources/Js/jquery.multiselect.filter.js"); }
      set { ViewState["ScriptUrl"] = value; }
    }

    public string FilterStyleSheetUrl
    {
      get { return (string)(ViewState["StyleSheetUrl"] ?? "{ManagementUrl}/Resources/Css/jquery.multiselect.filter.css"); }
      set { ViewState["StyleSheetUrl"] = value; }
    }

    private void RegisterClientScript()
    {
      Page.JQuery();
      Page.JQueryUi();
      Page.JavaScript(ScriptUrl);
      Page.StyleSheet(StyleSheetUrl);

      if (EnableFilter)
      {
        Page.JavaScript(FilterScriptUrl);
        Page.StyleSheet(FilterStyleSheetUrl);
      }


      var filter = EnableFilter ? ".multiselectfilter()" : "";

      var options = new List<string>
                      {
                        string.Format("selectedList: {0}", SelectedList)
                      };
      var optionsString = string.Join(",", options.ToArray());

      Page.JavaScript(string.Format("$('#{0}').multiselect({{{2}}}){1};", ClientID, filter, optionsString), ScriptPosition.Bottom, ScriptOptions.DocumentReady | ScriptOptions.ScriptTags);
    }

  }

}
