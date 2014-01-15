using System;
using System.Web.UI.WebControls;
using N2.Resources;
using N2.Templates.Web.UI;

namespace N2.Templates.UI.Views
{
    public partial class CalendarList : TemplatePage<N2.Templates.Items.Calendar>
    {
        protected Repeater rc;

        protected string RequestedDate
        {
            get { return Server.UrlDecode(Request["date"]); }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Register.StyleSheet(Page, "~/Templates/UI/Css/Calendar.css", Media.All);

            if(Request["filter"] != null)
                rc.DataSource = CurrentPage.GetEvents(Convert.ToDateTime(RequestedDate));
            else
                rc.DataSource = CurrentPage.GetEvents();
            rc.DataBind();
        }
    }
}
