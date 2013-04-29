using N2.Collections;
using N2.Edit;
using N2.Edit.Trash;
using N2.Engine;
using N2.Engine.Globalization;
using N2.Persistence;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Api
{
	public class ContextData
	{
		public ILanguage Language { get; set; }
	}

	public class Context : IHttpHandler
	{
		private SelectionUtility selection;
		private IEngine engine;

		public void ProcessRequest(HttpContext context)
		{
			engine = N2.Context.Current;
			selection = new SelectionUtility(context.Request, engine);

			context.Response.ContentType = "application/json";

			var ctx = new ContextData
			{
				Language = engine.Resolve<ILanguageGateway>().GetLanguage(selection.SelectedItem)
			};
			ctx.ToJson(context.Response.Output);
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}