using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Persistence;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace N2.Management.Api
{
	public class Context : IHttpHandler
	{
		private SelectionUtility selection;
		private IEngine engine;

		public void ProcessRequest(HttpContext context)
		{
			engine = N2.Context.Current;
			selection = new SelectionUtility(context.Request, engine);

			var item = selection.ParseSelectionFromRequest();

			var selectedUrl = context.Request["selectedUrl"];
			if (item == null && selectedUrl != null)
				item = selection.ParseUrl(selectedUrl);

			switch (context.Request.PathInfo)
			{
				case "/interface":
					context.Response.WriteJson(new InterfaceBuilder(engine).GetInterfaceContextData(new HttpContextWrapper(context), selection));
					return;
				case "/full":
					context.Response.WriteJson(new
					{
						Interface = new InterfaceBuilder(engine).GetInterfaceContextData(new HttpContextWrapper(context), selection),
						Context = new ContextBuilder(engine).GetInterfaceContextData(item, selectedUrl)
					});
					return;
				default:
					context.Response.WriteJson(new ContextBuilder(engine).GetInterfaceContextData(item, selectedUrl));
					return;
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}