using System;
using N2.Web;

namespace N2.Edit.Web
{
    public class UrlSelectionPage : EditPage
    {
        /// <summary>Gets the client id of the input control that should get the selected url.</summary>
        protected string OpenerInputId
        {
            get { return Request.QueryString["tbid"]; }
        }

        /// <summary>Gets the selected url from the calling page.</summary>
        protected string OpenerInputUrl
        {
            get { return Request.QueryString[SelectionUtility.SelectedQueryKey + "Url"]; }
        }

        /// <summary>Gets wether the dialog is to return a value to an input in the opener window.</summary>
        protected bool IsOpened
        {
            get { return !string.IsNullOrEmpty(OpenerInputId); }
        }

        protected N2.Web.UI.WebControls.UrlSelectorMode DefaultMode
        {
            get { return string.IsNullOrEmpty(Request.QueryString["defaultMode"]) ? N2.Web.UI.WebControls.UrlSelectorMode.Files : (N2.Web.UI.WebControls.UrlSelectorMode)Enum.Parse(typeof(N2.Web.UI.WebControls.UrlSelectorMode), Request.QueryString["defaultMode"]); }
        }
        protected N2.Web.UI.WebControls.UrlSelectorMode AvailableModes
        {
            get { return string.IsNullOrEmpty(Request.QueryString["availableModes"]) ? N2.Web.UI.WebControls.UrlSelectorMode.Files : (N2.Web.UI.WebControls.UrlSelectorMode)Enum.Parse(typeof(N2.Web.UI.WebControls.UrlSelectorMode), Request.QueryString["availableModes"]); }
        }
        protected bool AllModesAvailable
        {
            get { return AvailableModes == N2.Web.UI.WebControls.UrlSelectorMode.All; }
        }
        protected string AppendQueryString(string baseUrl)
        {
            return baseUrl += "&tbid=" + OpenerInputId
                + "&redirect=false"
                + "&defaultMode=" + DefaultMode 
                + "&availableModes=" + AvailableModes
                + "&selectedUrl=" + Server.UrlEncode(OpenerInputUrl);
        }
        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string scriptFormat = @"
var openerInputId = '{0}';
var isOpened = {1};
";
            string script = string.Format(scriptFormat, OpenerInputId, IsOpened.ToString().ToLower());
            ClientScript.RegisterStartupScript(typeof(UrlSelectionPage), "OnLoad", script, true);
        }
    }
}
