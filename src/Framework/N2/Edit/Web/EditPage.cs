using System;
using System.Resources;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Engine;
using N2.Resources;
using N2.Security;
using N2.Web;

namespace N2.Edit.Web
{
	/// <summary>
	/// Base class for edit mode pages. Provides functionality to parse 
	/// selected item and refresh navigation.
	/// </summary>
    public class EditPage : Page, IProvider<IEngine>
    {
		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			SetupAspNetTheming();
			Authorize(User);
		}

		/// <summary>Determines whether the current page can be displayed.</summary>
		/// <param name="user">The user to authorize.</param>
		/// <returns>True if the user is authorized.</returns>
		protected virtual void Authorize(IPrincipal user)
		{
			if(Engine.SecurityManager.IsAuthorized(user, Permission.Write))
				Engine.Resolve<ISecurityEnforcer>().AuthorizeRequest(user, Selection.SelectedItem, Permission.Read);
			else
				Engine.Resolve<ISecurityEnforcer>().AuthorizeRequest(user, Selection.SelectedItem, Permission.Write);
		}
	
		protected override void OnInit(EventArgs e)
		{
			EnsureAuthorization(Permission.Read);
			RegisterScripts();
			RegisterToolbarSelection();
			RegisterThemeCss();
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);

            base.OnInit(e);
		}

        private void RegisterThemeCss()
        {
            string theme = "default.css";
            var themeCookie = Request.Cookies["TH"];
            if (themeCookie != null && !string.IsNullOrEmpty(themeCookie.Value))
                theme = themeCookie.Value;

			Register.StyleSheet(this, Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/Css/themes/" + theme));
        }

		private void SetupAspNetTheming()
		{
			// asp.net themes are a bit cumbersome to work and deploy 
			// so I think this is going to be deprecated some time in the future
			if (EnableTheming)
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
		}

		/// <summary>Selects a toolbar item in the top frame</summary>
		protected virtual void RegisterToolbarSelection()
		{
			foreach (ToolbarPluginAttribute toolbarPlugin in GetType().GetCustomAttributes(typeof(ToolbarPluginAttribute), true))
			{
				string script = GetToolbarSelectScript(toolbarPlugin.Name);
				Register.JavaScript(this, script, ScriptPosition.Bottom, ScriptOptions.ScriptTags);
			}
		}

		protected virtual string GetToolbarSelectScript(string toolbarPluginName)
		{
			return string.Format("if(n2ctx){{ n2ctx.select('{0}'); jQuery(window).unload(function(){{n2ctx.unselect('{0}');}}); }}", toolbarPluginName);
		}

		protected virtual string CancelUrl()
        {
            if(!string.IsNullOrEmpty(Request["returnUrl"]))
                return Request["returnUrl"];
			var item = Selection.SelectedItem.VersionOf ?? Selection.SelectedItem;
			return NodeAdapter(item).GetPreviewUrl(item);
        }

		private NodeAdapter NodeAdapter(ContentItem item)
		{
			return Engine.GetContentAdapter<NodeAdapter>(item);
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
		protected virtual void EnsureAuthorization(Permission permission)
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

		protected new string ResolveUrl(string url)
		{
			return Engine.ManagementPaths.ResolveResourceUrl(url);
		}

		protected string ResolveUrl(object url)
		{
			return ResolveUrl(url as string);
		}

		protected string MapCssUrl(string cssFileName)
		{
			return Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/Css/" + cssFileName);
		}

    	#region Refresh Methods
		private const string RefreshBothFormat = @"if(window.n2ctx) n2ctx.refresh({{ navigationUrl:'{1}', previewUrl:'{2}', path:'{4}', permission:'{5}' }});";
		private const string RefreshNavigationFormat = @"if(window.n2ctx) n2ctx.refresh({{ navigationUrl:'{1}', path:'{4}', permission:'{5}' }});";
		private const string RefreshPreviewFormat = @"if(window.n2ctx) n2ctx.refresh({{ previewUrl: '{2}', path:'{4}', permission:'{5}' }});";

        protected virtual void Refresh(ContentItem item)
        {
            string previewUrl = Engine.ManagementPaths.GetEditInterfaceUrl(Selection.SelectedItem);
            string script = string.Format("window.top.location = '{0}';", previewUrl);

            ClientScript.RegisterClientScriptBlock(
                typeof(EditPage),
                "RefreshScript",
                script, true);
        }

        protected virtual void Refresh(ContentItem item, string previewUrl)
        {
            string script = string.Format(RefreshBothFormat,
                Engine.ManagementPaths.GetEditInterfaceUrl(), // 0
                GetNavigationUrl(item), // 1
                Url.ToAbsolute(previewUrl), // 2
                item.ID, // 3
                item.Path, // 4
				NodeAdapter(item).GetMaximumPermission(item)
            );

            ClientScript.RegisterClientScriptBlock(
                typeof(EditPage),
                "RefreshFramesScript",
                script, true);
        }

        /// <summary>Referesh the selected frames after loading the page.</summary>
        /// <param name="item"></param>
        /// <param name="area"></param>
		protected virtual void Refresh(ContentItem item, ToolbarArea area)
		{
			string script = GetRefreshScript(item, area);

			ClientScript.RegisterClientScriptBlock(
				typeof(EditPage),
				"RefreshFramesScript",
				script, true);
		}

		protected string GetRefreshScript(ContentItem item, ToolbarArea area)
		{
			string format;
			if (area == ToolbarArea.Both)
				format = RefreshBothFormat;
			else if (area == ToolbarArea.Preview)
				format = RefreshPreviewFormat;
			else
				format = RefreshNavigationFormat;

			string script = string.Format(format,
				Engine.ManagementPaths.GetEditInterfaceUrl(), // 0
				GetNavigationUrl(item), // 1
				GetPreviewUrl(item), // 2
				item.ID, // 3
				item.Path, // 4
				NodeAdapter(item).GetMaximumPermission(item)
				);
			return script;
		}

		protected string GetNavigationUrl(ContentItem selectedItem)
		{
			return Engine.ManagementPaths.GetNavigationUrl(selectedItem);
		}

		protected virtual string GetPreviewUrl(ContentItem selectedItem)
		{
			return Request["returnUrl"] ?? NodeAdapter(selectedItem).GetPreviewUrl(selectedItem);
		}
		#endregion

		#region Setup Toolbar Methods
		const string UpdateToolbarScript = "n2ctx.update({{ path:'{0}', id:'{1}', previewUrl:'{2}', permission:'{3}'}});";

		protected virtual void RegisterSetupToolbarScript(ContentItem item)
		{
			string script = string.Format(UpdateToolbarScript, item.Path, item.ID, GetPreviewUrl(item), NodeAdapter(item).GetMaximumPermission(item));
			ClientScript.RegisterClientScriptBlock(typeof(EditPage), "AddSetupToolbarScript", script, true);
		}

		#endregion

		#region Get Resource Methods
		protected string GetLocalResourceString(string resourceKey, string defaultText = null)
		{
			try
			{
				return (string)GetLocalResourceObject(resourceKey);
			}
			catch (InvalidOperationException)
			{
				return defaultText;
			}
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
		protected virtual void SetErrorMessage(BaseValidator validator, N2.Integrity.NameOccupiedException ex)
		{
			Trace.Write(ex.ToString());

			string message = string.Format(GetLocalResourceString("NameOccupiedExceptionFormat", "An item named \"{0}\" already exists below \"{1}\""),
				ex.SourceItem.Name,
				ex.DestinationItem.Name);
			SetErrorMessage(validator, message);
		}

		protected void SetErrorMessage(BaseValidator validator, N2.Integrity.DestinationOnOrBelowItselfException ex)
		{
			Trace.Write(ex.ToString());

			string message = string.Format(GetLocalResourceString("DestinationOnOrBelowItselfExceptionFormat", "Cannot move an item to a destination onto or below itself"),
				ex.SourceItem.Name,
				ex.DestinationItem.Name);
			SetErrorMessage(validator, message);
		}
		protected void SetErrorMessage(BaseValidator validator, N2.Definitions.NotAllowedParentException ex)
		{
			Trace.Write(ex.ToString());

			string message = string.Format(GetLocalResourceString("NotAllowedParentExceptionFormat", "The item of type \"{0}\" isn't allowed below a destination of type \"{1}\""),
				ex.ItemDefinition.Title,
				Engine.Definitions.GetDefinition(ex.ParentType).Title);
			SetErrorMessage(validator, message);
		}

		protected void SetErrorMessage(BaseValidator validator, Exception exception)
		{
            Engine.Resolve<IErrorNotifier>().Notify(exception);

			SetErrorMessage(validator, exception.Message);
		}

		protected void SetErrorMessage(BaseValidator validator, string message)
		{
			validator.IsValid = false;
			validator.ErrorMessage = message;
		}

		#endregion

        #region Obsolete
        [Obsolete("Don't use")]
		protected string Path
		{
			get { return Request["root"] ?? "/"; }
		}

        [Obsolete("Don't use")]
        protected virtual INode SelectedNode
        {
            get { return Selection.SelectedItem as INode; }
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
			get { return Selection.MemorizedItem; }
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
        public SelectionUtility Selection
        {
            get { return selection ?? (selection = new SelectionUtility(this, Engine)); }
            set { selection = value; }
        }

		[Obsolete("Don't use", true)]
		protected ContentItem RootNode
		{
			get { return Engine.Resolve<Navigator>().Navigate(Path); }
		}

		#endregion

		#region IProvider<IEngine> Members

		IEngine IProvider<IEngine>.Get()
		{
			return Engine;
		}

		System.Collections.Generic.IEnumerable<IEngine> IProvider<IEngine>.GetAll()
		{
			return Engine.Container.ResolveAll<IEngine>();
		}

		#endregion
    }
}
