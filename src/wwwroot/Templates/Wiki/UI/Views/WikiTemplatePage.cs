using System;
using N2.Templates.Web.UI;

namespace N2.Templates.Wiki.UI.Views
{
	public class WikiTemplatePage : TemplatePage<Items.WikiArticle>
	{
		protected bool IsAuthorized
		{
			get
			{
				foreach (string role in CurrentPage.WikiRoot.ModifyRoles)
					if (User.IsInRole(role))
						return true;
				return false;
			}
		}

		protected string CurrentAction
		{
			get { return Engine.RequestContext.CurrentTemplate.Action; }
		}

		protected string CurrentArguments
		{
			get { return Engine.RequestContext.CurrentTemplate.Arguments; }
		}

		protected override void OnPreRender(EventArgs e)
		{
			N2.Resources.Register.StyleSheet(this, "~/Templates/Wiki/UI/Css/Wiki.css");
			base.OnPreRender(e);
		}
	}
}