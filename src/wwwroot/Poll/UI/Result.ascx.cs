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
using N2.Templates.Survey.Items;

namespace N2.Templates.Poll.UI
{
	public partial class Result : Templates.Web.UI.TemplateUserControl<Templates.Items.AbstractContentPage, Items.Poll>
	{
		private int total = 0;

		public int Total
		{
			get { return total; }
			set { total = value; }
		}
	
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Resources.Register.StyleSheet(Page, "~/Poll/UI/Css/Poll.css", N2.Resources.Media.All);

			LoadTotal();

			rptAnswers.DataSource = CurrentItem.Question.Options;
			rptAnswers.DataBind();
		}

		private void LoadTotal()
		{
			if (CurrentItem.Question != null)
			{
				foreach (Option o in CurrentItem.Question.Options)
				{
					Total += o.Answers;
				}
			}
			if (Total == 0)
				Total = 100;
		}
	}
}