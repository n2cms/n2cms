using System;
using System.Security.Principal;
using System.Web.UI;
using N2.Engine;
using N2.Plugin;
using System.Web.UI.WebControls;

namespace N2.Edit
{
	/// <summary>
	/// Classes extending this abstract class are collected and may be 
	/// retrieved by user interfaces in the editing interface.
	/// </summary>
	public abstract class AdministrativePluginAttribute : Attribute, IPlugin
	{
		private string[] authorizedRoles;
		private bool enabled = true;
		private string name;
		private int sortOrder = int.MaxValue;
		private Type decorates;
		private IEngine engine;

		#region Public Properties

		public Type Decorates
		{
			get { return decorates; }
			set { decorates = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public int SortOrder
		{
			get { return sortOrder; }
			set { sortOrder = value; }
		}

		public string[] AuthorizedRoles
		{
			get { return authorizedRoles; }
			set { authorizedRoles = value; }
		}

		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		/// <summary>Url to the anchor's image icon.</summary>
		public string IconUrl { get; set; }

		/// <summary>The current N2 context.</summary>
		protected internal IEngine Engine
		{
			get { return engine ?? (engine = N2.Context.Current); }
			set { engine = value; }
		}

		#endregion

		/// <summary>Find out whether a user has permission to view this plugin in the toolbar.</summary>
		/// <param name="user">The user to check.</param>
		/// <returns>True if the user is null or no permissions are required or the user has permissions.</returns>
		public bool IsAuthorized(IPrincipal user)
		{
			if (user == null || authorizedRoles == null)
				return true;
			foreach (string role in authorizedRoles)
				if (string.Equals(user.Identity.Name, role, StringComparison.OrdinalIgnoreCase) || user.IsInRole(role))
					return true;
			return false;
		}

		protected string GetInnerHtml(PluginContext pluginContext, string iconUrl, string alt, string text)
		{
			if (string.IsNullOrEmpty(iconUrl))
				return text;
			return string.Format("<img src='{0}' alt='{1}'/>{2}", pluginContext.Rebase(iconUrl), alt, text);
		}

		protected virtual void ApplyStyles(PluginContext pluginContext, WebControl ctrl)
		{
			if (!string.IsNullOrEmpty(IconUrl))
				ctrl.Style[HtmlTextWriterStyle.BackgroundImage] = pluginContext.Rebase(IconUrl);
		}

		public abstract Control AddTo(Control container, PluginContext context);

        #region IComparable<IPlugin> Members

        public int CompareTo(IPlugin other)
		{
			if(other == null)
				return 1;
			int result = SortOrder.CompareTo(other.SortOrder) * 2 + Name.CompareTo(other.Name);
			return result;
		}

		#region Equals & GetHashCode Methods

		public override bool Equals(object obj)
		{
			if (obj == null || !obj.GetType().IsAssignableFrom(GetType()))
				return false;
			return Name == ((AdministrativePluginAttribute) obj).Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion

		#endregion
	}
}