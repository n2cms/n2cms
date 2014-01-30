using System;
using System.Web;
using N2.Edit.FileSystem;
using N2.Edit.Web;

namespace N2.Edit.Settings
{
    public partial class SettingsEditor : EditUserControl, ISettingsEditor
    {
        NavigationSettings settings;

        protected override void OnInit(EventArgs e)
        {
            settings = Engine.Resolve<NavigationSettings>();
            chkShowDataItems.Checked = settings.DisplayDataItems;
            ddlThemes.DataSource = Engine.Resolve<IFileSystem>().GetFiles(Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/Css/themes"));
            ddlThemes.DataBind();
            ddlThemes.SelectedValue = GetOrAddCookie(Request.Cookies, "default.css").Value;

            base.OnInit(e);
        }

        public void Save()
        {
            settings.DisplayDataItems = chkShowDataItems.Checked;
            GetOrAddCookie(Response.Cookies, "default.css").Value = ddlThemes.SelectedValue;
        }

        private static HttpCookie GetOrAddCookie(HttpCookieCollection cookies, string defaultValue)
        {
            HttpCookie ddi = cookies["TH"];
            if (ddi == null)
            {
                ddi = new HttpCookie("TH", defaultValue);
                cookies.Add(ddi);
            }
            return ddi;
        }
    }
}
