using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using N2.Resources;
using N2.Web;

namespace N2.Edit.Web
{
	/// <summary>
	/// Base class for edit mode pages. Provides functionality to parse 
	/// selected item and refresh navigation.
	/// </summary>
    public class EditPage : Page
    {
		protected override void OnInit(EventArgs e)
		{
			RegisterScripts();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
			base.OnInit(e);
		}

		protected virtual void RegisterScripts()
		{
			Register.JQuery(this);
			Register.JavaScript(this, "~/Edit/Js/Plugins.ashx?v=2");

			// select toolbar
			foreach(ToolbarPluginAttribute toolbarPlugin in GetType().GetCustomAttributes(typeof(ToolbarPluginAttribute), true))
			{
				if (toolbarPlugin.GetType() != typeof(ToolbarPluginAttribute))
					continue;

				string script = GetToolbarSelectScript(toolbarPlugin);
				Register.JavaScript(this, script, ScriptPosition.Bottom, ScriptOptions.ScriptTags);
			}
		}

		protected virtual string GetToolbarSelectScript(ToolbarPluginAttribute toolbarPlugin)
		{
			return string.Format("n2ctx.toolbarSelect('{0}');", toolbarPlugin.Name);
		}

		protected virtual string CancelUrl()
        {
            return Request["returnUrl"] ?? (SelectedItem.VersionOf ?? SelectedNode).PreviewUrl;
        }

    	#region Refresh Methods
		private const string RefreshBothFormat = @"
n2ctx.setupToolbar('{4}'); 
n2ctx.refresh('{1}', '{2}');
";

		private const string RefreshNavigationFormat = @"
n2ctx.setupToolbar('{4}'); 
n2ctx.refreshNavigation('{1}', '{2}');
";
		private const string RefreshPreviewFormat = @"
n2ctx.setupToolbar('{4}'); 
n2ctx.refreshPreview('{1}', '{2}');
";

		protected virtual void Refresh(ContentItem item, N2.Edit.ToolbarArea area)
		{
			string format;
			if (area == ToolbarArea.Both)
				format = RefreshBothFormat;
			else if (area == ToolbarArea.Preview)
				format = RefreshPreviewFormat;
			else
				format = RefreshNavigationFormat;

			string script = string.Format(format,
				N2.Web.Url.ToAbsolute("~/Edit/Default.aspx"), // 0
				GetNavigationUrl(item), // 1
				GetPreviewUrl(item), // 2
				item.ID, // 3
				item.Path // 4
				);

			ClientScript.RegisterClientScriptBlock(
				typeof(EditPage),
				"AddRefreshEditScript",
				script, true);
		}

		protected string GetNavigationUrl(ContentItem selectedItem)
		{
			return Engine.EditManager.GetNavigationUrl(selectedItem);
		}

		protected virtual string GetPreviewUrl(ContentItem selectedItem)
		{
			return Request["returnUrl"] ?? Engine.EditManager.GetPreviewUrl(selectedItem);
		}
		#endregion

		#region Setup Toolbar Methods
		protected virtual string SetupToolbarScriptFormat
		{
			get { return "window.n2ctx.setupToolbar('{0}');"; }
		}

		protected virtual void RegisterSetupToolbarScript(ContentItem item)
		{
			string script = string.Format(SetupToolbarScriptFormat, item.Path, item.ID);
			ClientScript.RegisterClientScriptBlock(typeof(EditPage), "AddSetupToolbarScript", script, true);
		}

		#endregion

		#region Get Resource Methods
		protected string GetLocalResourceString(string resourceKey)
		{
			return (string)GetLocalResourceObject(resourceKey);
		}
		protected string GetGlobalResourceString(string className, string resourceKey)
		{
            try
            {
                return (string)GetGlobalResourceObject(className, resourceKey);
            }
            catch(MissingManifestResourceException)
            {
                return null;
            }
		} 

		#endregion

		#region Error Handling
		protected void SetErrorMessage(BaseValidator validator, N2.Integrity.NameOccupiedException ex)
		{
			Trace.Write(ex.ToString());

			string message = string.Format(GetLocalResourceString("NameOccupiedExceptionFormat"),
				ex.SourceItem.Name,
				ex.DestinationItem.Name);
			SetErrorMessage(validator, message);
		}

		protected void SetErrorMessage(BaseValidator validator, N2.Integrity.DestinationOnOrBelowItselfException ex)
		{
			Trace.Write(ex.ToString());

			string message = string.Format(GetLocalResourceString("DestinationOnOrBelowItselfExceptionFormat"),
				ex.SourceItem.Name,
				ex.DestinationItem.Name);
			SetErrorMessage(validator, message);
		}
		protected void SetErrorMessage(BaseValidator validator, N2.Definitions.NotAllowedParentException ex)
		{
			Trace.Write(ex.ToString());

			string message = string.Format(GetLocalResourceString("NotAllowedParentExceptionFormat"),
				ex.ItemDefinition.Title,
				Engine.Definitions.GetDefinition(ex.ParentType).Title);
			SetErrorMessage(validator, message);
		}

		protected void SetErrorMessage(BaseValidator validator, Exception exception)
		{
            Engine.Resolve<IErrorHandler>().Notify(exception);

			SetErrorMessage(validator, exception.Message);
		}

		protected void SetErrorMessage(BaseValidator validator, string message)
		{
			validator.IsValid = false;
			validator.ErrorMessage = message;
		}

		#endregion

		#region Properties

		protected string Path
		{
			get { return Request["root"] ?? "/"; }
		}

		protected ContentItem RootNode
		{
			get { return Engine.Resolve<Navigator>().Navigate(Path); }
		}

		public virtual Engine.IEngine Engine
		{
			get { return N2.Context.Current; }
		}

        public N2.Web.HtmlHelper Html
        {
            get { return new N2.Web.HtmlHelper(this, SelectedItem); }
        }

		public override string ID
		{
			get { return base.ID ?? "P"; }
		}

		private int SelectedItemID
		{
			get { return (int)(ViewState["SelectedItemID"] ?? 0); }
			set { ViewState["SelectedItemID"] = value; }
		}

		protected virtual INode SelectedNode
		{
			get { return SelectedItem as INode; }
		}
		
		/// <summary>Gets the currently selected item by the tree menu in edit mode.</summary>
		public virtual ContentItem SelectedItem
		{
			get
			{
				ContentItem selectedItem = GetFromViewState()
					?? GetFromUrl()
					?? Engine.UrlParser.StartPage;
				return selectedItem;
			}
			set
			{
				if (value != null)
					SelectedItemID = value.ID;
				else
					SelectedItemID = 0;
			}
		}

		private ContentItem memorizedItem = null;
		protected ContentItem MemorizedItem
		{
			get { return memorizedItem ?? (memorizedItem = Engine.Resolve<Navigator>().Navigate(Request.QueryString["memory"])); }
		}

		#endregion

		#region Helper Methods

		private ContentItem GetFromViewState()
		{
			if (SelectedItemID != 0)
				return Engine.Persister.Get(SelectedItemID);
			return null;
		}

		private ContentItem GetFromUrl()
		{
			string selected = GetSelectedPath();
			if (!string.IsNullOrEmpty(selected))
				return Engine.Resolve<Navigator>().Navigate(selected);

			string selectedUrl = Request["selectedUrl"];
			if (!string.IsNullOrEmpty(selectedUrl))
				return Engine.UrlParser.Parse(selectedUrl);

			string itemId = Request[PathData.ItemQueryKey];
			if (!string.IsNullOrEmpty(itemId))
				return Engine.Persister.Get(int.Parse(itemId));

			return null;
		}

		protected string GetSelectedPath()
		{
			return Request["selected"];
		}

		#endregion
    }
}
