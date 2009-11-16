using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using N2.Resources;
using N2.Security;
using N2.Web;

namespace N2.Edit.Web
{
	/// <summary>
	/// Base class for edit mode pages. Provides functionality to parse 
	/// selected item and refresh navigation.
	/// </summary>
    public class EditPage : Page
    {
		private ContentItem selectedItem;
		private ContentItem memorizedItem = null;

		public EditPage()
		{
			PreInit += EditPage_PreInit;
		}

		void EditPage_PreInit(object sender, EventArgs e)
		{
			SetupAspNetTheming();
		}
		
		protected override void OnInit(EventArgs e)
		{
			RegisterScripts();
            RegisterThemeCss();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
			
            base.OnInit(e);
		}

        private void RegisterThemeCss()
        {
            string theme = "default.css";
            var themeCookie = Request.Cookies["TH"];
            if (themeCookie != null && !string.IsNullOrEmpty(themeCookie.Value))
                theme = themeCookie.Value;

            Register.StyleSheet(this, "~/edit/css/themes/" + theme);
        }

		private void SetupAspNetTheming()
		{            
            // asp.net themes are a bit cumbersome to work and deploy 
            // so I think this is going to be deprecated some time in the future
			if(EnableTheming)
				Theme = Engine.EditManager.EditTheme;
		}

		public override bool EnableTheming
		{
			get { return !String.IsNullOrEmpty(Engine.EditManager.EditTheme); }
			set { base.EnableTheming = value; }
		}

		protected virtual void RegisterScripts()
		{
			Register.JQuery(this);
			Register.JQueryPlugins(this);

			// select toolbar
			foreach(ToolbarPluginAttribute toolbarPlugin in GetType().GetCustomAttributes(typeof(ToolbarPluginAttribute), true))
			{
				string script = GetToolbarSelectScript(toolbarPlugin);
				Register.JavaScript(this, script, ScriptPosition.Bottom, ScriptOptions.ScriptTags);
			}
		}

		protected virtual string GetToolbarSelectScript(ToolbarPluginAttribute toolbarPlugin)
		{
			return string.Format("if(window.n2ctx)window.n2ctx.toolbarSelect('{0}');", toolbarPlugin.Name);
		}

		protected virtual string CancelUrl()
        {
            return Request["returnUrl"] ?? (Selection.SelectedItem.VersionOf ?? SelectedNode).PreviewUrl;
        }

		/// <summary>Checks that the user has the required permission on the given item. Throws exceptions if authorization is missing.</summary>
		/// <param name="item">The item to check permissions on.</param>
		/// <param name="permission">The permission to check.</param>
		protected void EnsureAuthorization(ContentItem item, Permission permission)
		{
			if (!IsAuthorized(item, permission))
				throw new PermissionDeniedException(item, User);
		}

		/// <summary>Checks that the user has the required permission on the selected item. Throws exceptions if authorization is missing.</summary>
		/// <param name="permission">The permission to check.</param>
		protected void EnsureAuthorization(Permission permission)
		{
            EnsureAuthorization(Selection.SelectedItem, permission);
		}

		/// <summary>Checks that the user has the required permission on the given item.</summary>
		/// <param name="item">The item to check permissions on.</param>
		/// <param name="permission">The permission to check.</param>
		protected bool IsAuthorized(ContentItem item, Permission permission)
		{
			if(Engine.SecurityManager.IsAdmin(User))
				return true;
			return Engine.SecurityManager.IsAuthorized(User, item, permission);
		}

		/// <summary>Checks that the user has the required permission on the selected item.</summary>
		/// <param name="permission">The permission to check.</param>
		protected bool IsAuthorized(Permission permission)
		{
            return IsAuthorized(Selection.SelectedItem, permission);
		}

		protected string MapCssUrl(string cssFileName)
		{
			return Url.ToAbsolute(N2.Context.Current.EditManager.GetEditInterfaceUrl() + "Css/" + cssFileName);
		}

    	#region Refresh Methods
		private const string RefreshBothFormat = @"
if(window.n2ctx){{
	window.n2ctx.setupToolbar('{4}','{2}'); 
	window.n2ctx.refresh('{1}', '{2}');
}}";

		private const string RefreshNavigationFormat = @"
if(window.n2ctx){{
	window.n2ctx.setupToolbar('{4}','{2}'); 
	window.n2ctx.refreshNavigation('{1}', '{2}');
}}";
		private const string RefreshPreviewFormat = @"
if(window.n2ctx){{
	window.n2ctx.setupToolbar('{4}','{2}'); 
	window.n2ctx.refreshPreview('{1}', '{2}');
}}";

		protected virtual void Refresh(ContentItem item, ToolbarArea area)
		{
			string format;
			if (area == ToolbarArea.Both)
				format = RefreshBothFormat;
			else if (area == ToolbarArea.Preview)
				format = RefreshPreviewFormat;
			else
				format = RefreshNavigationFormat;

			string script = string.Format(format,
				Url.ToAbsolute("~/Edit/Default.aspx"), // 0
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
			get { return "if(window.n2ctx)window.n2ctx.setupToolbar('{0}','{2}');"; }
		}

		protected virtual void RegisterSetupToolbarScript(ContentItem item)
		{
			string script = string.Format(SetupToolbarScriptFormat, item.Path, item.ID, GetPreviewUrl(item));
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

        #region Obsolete
        [Obsolete]
		protected string Path
		{
			get { return Request["root"] ?? "/"; }
		}

        [Obsolete]
		protected ContentItem RootNode
		{
			get { return Engine.Resolve<Navigator>().Navigate(Path); }
		}

        [Obsolete]
        public N2.Web.HtmlHelper Html
        {
            get { return new N2.Web.HtmlHelper(this, Selection.SelectedItem); }
        }

        [Obsolete]
        protected virtual INode SelectedNode
        {
            get { return Selection.SelectedItem as INode; }
        }
        #endregion

        #region Properties

        Engine.IEngine engine;
        public Engine.IEngine Engine
        {
            get { return engine ?? (engine = N2.Context.Current); }
            set { engine = value; }
        }

        SelectionUtility selection;
        protected SelectionUtility Selection
        {
            get { return selection ?? (selection = new SelectionUtility(this, Engine)); }
            set { selection = value; }
        }

		/// <summary>Gets the currently selected item by the tree menu in edit mode.</summary>
        [Obsolete("Use Selection.SelectedItem")]
		public virtual ContentItem SelectedItem
		{
			get { return Selection.SelectedItem; }
			set { Selection.SelectedItem = value; }
		}

        [Obsolete("Use Selection.MemorizedItem")]
        protected ContentItem MemorizedItem
		{
			get { return memorizedItem ?? (memorizedItem = Engine.Resolve<Navigator>().Navigate(Request.QueryString["memory"])); }
		}

		#endregion
    }
}
