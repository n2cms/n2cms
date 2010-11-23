using System;
using System.IO;
using System.Web.UI;
using N2.Resources;
using N2.Web;

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

		protected override void OnPreRender(EventArgs e)
		{
			if (h1 != null)
			{
				h1.InnerHtml = Page.Title;
				h1.Visible = !string.IsNullOrEmpty(Page.Title);
			}
			base.OnPreRender(e);
		}

		protected string MapCssUrl(string cssFileName)
		{
			return Url.ToAbsolute(N2.Context.Current.EditUrlManager.GetEditInterfaceUrl() + "../Resources/Css/" + cssFileName);
		}
	}
}