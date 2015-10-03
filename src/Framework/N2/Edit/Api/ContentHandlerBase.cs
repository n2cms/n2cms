using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Api
{
	public abstract class ContentHandlerBase
	{
		public virtual string HttpMethod { get { return "GET"; } }
		public virtual string PathInfo 
		{ 
			get 
			{ 
				string name = GetType().Name;
				return name.EndsWith("ContentHandler")
						? "/" + name.Substring(0, name.Length - "ContentHandler".Length).ToLower()
						: "/" + name.ToLower(); 
			} 
		}

		public virtual bool Handle(System.Web.HttpContextBase context)
		{
			if (context.Request.HttpMethod != HttpMethod)
				return false;
			if (context.Request.PathInfo != PathInfo)
				return false;

			var result = HandleDataRequest(context);
			if (result == null)
				return false;
			
			context.Response.WriteJson(result);
			return true;
		}

		protected virtual object HandleDataRequest(System.Web.HttpContextBase context)
		{
			return null;
		}
	}
}
