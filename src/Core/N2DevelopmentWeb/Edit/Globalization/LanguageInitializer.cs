using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Plugin;
using N2.Globalization;

namespace N2.Edit.Globalization
{
	[AutoInitialize]
	public class LanguageInitializer : IPluginInitializer
	{
		public void Initialize(N2.Engine.IEngine engine)
		{
			engine.AddComponent("n2.languageGateway", typeof(ILanguageGateway), typeof(LanguageGateway));
			engine.AddComponent("n2.languageInterceptor", typeof(LanguageInterceptor));
		}
	}
}
