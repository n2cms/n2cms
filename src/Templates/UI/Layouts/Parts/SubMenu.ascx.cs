using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace N2.Templates.UI.Layouts.Parts
{
	public partial class SubMenu : N2.Templates.Web.UI.TemplateUserControl<ContentItem>
	{
		public int StartLevel
		{
			get { return sm.StartLevel; }
			set { sm.StartLevel = value; }
		}

		protected override void OnInit(EventArgs e)
		{
			InitSubMenu();
			base.OnInit(e);
		}

		private void InitSubMenu()
		{
			ContentItem branchRoot = Find.FindAncestorAtLevel(2);
			if (branchRoot != null)
				hsm.Text = N2.Web.Link.To(branchRoot).ToString();
			else
				this.Visible = false;
		}
	}
}