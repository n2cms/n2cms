using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace N2.Edit.Api
{
	public abstract class ContentHandlerBase
	{
		public ContentHandlerBase()
		{
			string name = GetType().Name;
			PathInfo = name.EndsWith("ContentHandler")
					? "/" + name.Substring(0, name.Length - "ContentHandler".Length).ToLower()
					: "/" + name.ToLower(); 
		}

		public virtual string PathInfo { get; set; }

		public virtual bool Handle(System.Web.HttpContextBase context)
		{
			if (!context.Request.PathInfo.StartsWith(PathInfo))
				return false;

			try
			{
				var result = HandleDataRequest(context);
				if (result == null)
					return false;

				context.Response.WriteJson(result);
				return true;
			}
			catch (HttpException ex)
			{
				context.Response.Status = ex.GetHttpCode() + " " + ex.Message;
				return true;
			}
		}

		protected virtual object HandleDataRequest(System.Web.HttpContextBase context)
		{
			var action = context.Request.PathInfo.Substring(PathInfo.Length).Trim('/');
			if (string.IsNullOrEmpty(action))
				action = "Index";

			if (context.Request.HttpMethod != "GET")
				action = context.Request.HttpMethod[0] + context.Request.HttpMethod.Substring(1).ToLower() + action;

			var method = GetType().GetMethods().FirstOrDefault(m => m.Name.Equals(action, StringComparison.InvariantCultureIgnoreCase));
			if (method == null)
				throw new HttpException(404, action + " not found");

			var methodParameters = method.GetParameters()
				.Select(pi => GetParameterValue(context, pi))
				.ToArray();

			return method.Invoke(this, methodParameters);
		}

		private static object GetParameterValue(System.Web.HttpContextBase context, System.Reflection.ParameterInfo pi)
		{
			if (pi.Name == "body")
				return context.GetOrDeserializeRequestStreamJson(pi.ParameterType);
			if (pi.Name == "context")
				return context;
			var queryValue = context.Request[pi.Name];
			if (string.IsNullOrEmpty(queryValue))
				return null;
			return Utility.Convert(queryValue, pi.ParameterType);
		}
	}
}
