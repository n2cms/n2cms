using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2DevelopmentWeb.Domain
{
    [N2.Definition("My special page")]
    [N2.Integrity.RestrictParents(typeof(MyPageData))]
	[N2.Details.WithEditable("Expires", typeof(System.Web.UI.WebControls.TextBox), "Text", 80, "Expires", ContainerName="default")]
    [N2.Web.UI.EditorModifier("Width", "100px", Name = "Expires")]
    public class MySpecialPageData : MyPageData
    {
        [N2.Details.EditableTextBox("Published", 80, ContainerName="default")]
        [N2.Web.UI.EditorModifier("Width", "100")]
        [N2.Web.UI.EditorModifier("BackColor", "#ff9933")]
        public override DateTime? Published
        {
            get { return base.Published; } 
            set { base.Published = value; }
        }

		[N2.Details.EditableUserControl("A special date", "~/Uc/UCEditorTest.ascx", "SelectedDate", 90, ContainerName = "special")]
        public virtual DateTime SpecialDate
        {
            get { return (DateTime)(GetDetail("SpecialDate") ?? new DateTime(2006, 1, 1)); }
            set { SetDetail("SpecialDate", value); }
        }

		[N2.Details.Editable("A very special text", typeof(N2.Web.UI.WebControls.FreeTextArea), "Text", 120, ContainerName = "special")]
        public virtual string SpecialText
        {
            get { return (string)GetDetail("SpecialText"); }
            set { SetDetail("SpecialText", value); }
        }

        public override string TemplateUrl
        {
            get
            {
                return AlternativeTemplateUrl ?? "/Templates/Special.aspx";
            }
        }
    }
}
