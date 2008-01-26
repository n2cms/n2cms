using System;
using System.Web.UI;
using N2.Resources;

namespace N2.Edit
{
	public partial class Framed : MasterPage
	{
		public override string ID
		{
			get { return base.ID ?? "F"; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Register.JQuery(Page);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			title.Text = Page.Title;
			h1.InnerHtml = Page.Title;
		}
	}
}