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
using System.Linq;
using System.Web;

namespace N2.Management.Api
{
	public class ContextData
	{
		public ILanguage Language { get; set; }

		public TreeNode CurrentItem { get; set; }

		public List<string> Flags { get; set; }
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

			var item = selection.ParseSelectionFromRequest();
			var ctx = new ContextData();
			var selectedUrl = context.Request["selectedUrl"];

			if (item == null && selectedUrl != null)
				item = selection.ParseSelected(selectedUrl.ToUrl()[PathData.SelectedQueryKey]);
			
			if (item != null)
			{
				var adapter = engine.GetContentAdapter<NodeAdapter>(item);
				ctx.CurrentItem = adapter.GetTreeNode(item);
				ctx.Language = adapter.GetLanguage(item);
				ctx.Flags = adapter.GetNodeFlags(item).ToList();
			}
			else
				ctx.Flags = new List<string>();
			
			var mangementUrl = "{ManagementUrl}".ResolveUrlTokens();
			if (selectedUrl != null && selectedUrl.StartsWith(mangementUrl, StringComparison.InvariantCultureIgnoreCase))
			{
				ctx.Flags.Add("Management");
				ctx.Flags.Add(selectedUrl.Substring(mangementUrl.Length).ToUrl().PathWithoutExtension.Replace("/", ""));
			}
			
			ctx.ToJson(context.Response.Output);
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}