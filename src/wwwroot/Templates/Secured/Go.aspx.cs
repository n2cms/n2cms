using System;
using N2.Edit;

namespace N2.Templates.Secured
{
    [ToolbarPlugin("", "", "~/Templates/Secured/Go.aspx?selected={selected}", N2.Edit.ToolbarArea.Preview, "_top", "~/Templates/UI/Img/eye.png", 0, Name = "Go")]
    public partial class Go : N2.Edit.Web.EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            Response.Redirect(SelectedItem.Url);
        }
    }
}