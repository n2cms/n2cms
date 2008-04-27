using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Diagnostics;
using N2.Persistence;
using N2.Security;

namespace N2DevelopmentWeb
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
			Debug.WriteLine("Application_Start");
        }

        void UrlParser_PageNotFound(object sender, N2.Web.PageNotFoundEventArgs e)
        {
            //e.AffectedItem = N2.Context.Persister.Get(3);
        }

		public override void Init()
		{
			base.Init();
			Debug.WriteLine("Init");
			//log.Error("Init");
            N2.Context.UrlParser.PageNotFound += new EventHandler<N2.Web.PageNotFoundEventArgs>(UrlParser_PageNotFound);
			N2.Context.Current.Resolve<ISecurityEnforcer>().AuthorizationFailed += new EventHandler<CancellableItemEventArgs>(Global_AuthorizationFailed);
        }

		void Global_AuthorizationFailed(object sender, CancellableItemEventArgs e)
		{
			if(Context.Request["overrideSecurity"] == "true")
				e.Cancel = true;
		}

        protected void Application_End(object sender, EventArgs e)
        {
			Debug.WriteLine("Application_End");
			//log.Error("Application_End");
        }

		public override void Dispose()
		{
			Debug.WriteLine("Global.Dispose");
			base.Dispose();
		}

		~Global()
		{
			//Debug.WriteLine("~Global");
		}
    }
}
