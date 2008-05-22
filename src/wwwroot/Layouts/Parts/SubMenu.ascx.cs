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
using N2.Collections;

namespace N2.Templates.UI.Layouts.Parts
{
	public partial class SubMenu : N2.Templates.Web.UI.TemplateUserControl<ContentItem>
	{
		public ContentItem StartPage
		{
			get { return m.StartPage; }
			set { m.StartPage = value; }
		}

		public int StartLevel
		{
			get { return m.StartLevel; }
			set { m.StartLevel = value; }
		}

		protected override void OnInit(EventArgs e)
		{
			Page.InitComplete += InitMenu;

			base.OnInit(e);
		}

		private void InitMenu(object sender, EventArgs e)
		{
			ContentItem branchRoot = Find.AncestorAtLevel(StartLevel, Find.EnumerateParents(CurrentPage, StartPage), CurrentPage);

			if (branchRoot != null && branchRoot.GetChildren(new NavigationFilter()).Count > 0)
				hsm.Text = N2.Web.Link.To(branchRoot).ToString();
			else
				this.Visible = false;
		}
	}
}