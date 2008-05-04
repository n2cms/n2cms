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
using N2.Globalization;
using N2;

namespace N2DevelopmentWeb
{
    public partial class DefaultMasterPage : N2.Web.UI.MasterPage<N2.ContentItem>
    {
		protected override void OnInit(EventArgs e)
		{
			changepath.CurrentItem = N2.Context.Current.Resolve<ILanguageGateway>().GetLanguage(CurrentPage) as ContentItem;

			base.OnInit(e);
		}
    }
}
