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
	public class Translations : IHttpHandler
	{
		private SelectionUtility selection;
		private IEngine engine;

		public void ProcessRequest(HttpContext context)
		{
			engine = N2.Context.Current;
			selection = new SelectionUtility(context.Request, engine);

			context.Response.ContentType = "application/json";

			var translations = CreateTranslations(context).ToList();
			translations.ToJson(context.Response.Output);
		}

		private IEnumerable<Node<InterfaceMenuItem>> CreateTranslations(HttpContext context)
		{
			return engine.Resolve<ILanguageGateway>().GetEditTranslations(selection.SelectedItem, true, true)
				.Select(t => new Node<InterfaceMenuItem>(new InterfaceMenuItem
				{
					Title = t.Language.LanguageTitle,
					Url = t.EditUrl,
					IconUrl = t.FlagUrl
				}));
		}

		private IEnumerable<Node<TreeNode>> CreateChildren(HttpContext context)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(selection.SelectedItem);
			var filter = engine.EditManager.GetEditorFilter(context.User);
			return adapter.GetChildren(selection.SelectedItem, Interfaces.Managing)
				.Select(c => CreateNode(c, filter));
		}

		private Node<TreeNode> CreateNode(ContentItem item, ItemFilter filter)
		{
			var adapter = engine.GetContentAdapter<NodeAdapter>(item);
			return new Node<TreeNode>
			{
				Current = adapter.GetTreeNode(item),
				Children = new Node<TreeNode>[0],
				HasChildren	= adapter.HasChildren(item, filter),
				Expanded = false
			};
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}