using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2DevelopmentWeb.Uc
{
    public partial class UCEditorTest : System.Web.UI.UserControl
    {
        //protected override void OnPreRender(EventArgs e)
        //{
        //    base.OnPreRender(e);

        //    int y = int.Parse(year.SelectedValue);
        //    int m = int.Parse(month.SelectedValue);
        //    int d = int.Parse(day.SelectedValue);
        //}

        public DateTime SelectedDate
        {
            get
            {
                int y = int.Parse(year.SelectedValue);
                int m = int.Parse(month.SelectedValue);
                int d = int.Parse(day.SelectedValue);
                return new DateTime(y, m, d);
            }
            set
            {
                year.SelectedValue = value.Year.ToString();
                month.SelectedValue = value.Month.ToString();
                day.SelectedValue = value.Day.ToString();
            }
        }
    }
}