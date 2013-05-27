using N2.Collections;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Trash;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Persistence;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Api
{
	public class Definitions : IHttpHandler
	{
		private SelectionUtility selection;
		private IEngine engine;

		public void ProcessRequest(HttpContext context)
		{
			engine = N2.Context.Current;
			selection = new SelectionUtility(context.Request, engine);

			context.Response.ContentType = "application/json";
			
			engine.Definitions.GetAllowedChildren(selection.SelectedItem, null)
				.WhereAuthorized(engine.SecurityManager, context.User, selection.SelectedItem)
				.Select(d => new { d.Title, d.Description, d.Discriminator, d.ToolTip, d.IconUrl })
				.ToList().ToJson(context.Response.Output);
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}