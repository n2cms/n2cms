using System;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Edit
{
	public abstract class EditingPlugInAttribute : Attribute, IComparable<EditingPlugInAttribute>
	{
		private string[] authorizedRoles;
		private bool enabled = true;
		private string globalResourceClassName;
		private string iconUrl;
		private string name;
		private int sortOrder = int.MaxValue;
		private string target = "preview";
		private string title;
		private string toolTip;
		private string urlFormat;

		#region Public Properties

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string ToolTip
		{
			get { return toolTip; }
			set { toolTip = value; }
		}

		public int SortOrder
		{
			get { return sortOrder; }
			set { sortOrder = value; }
		}

		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		public string IconUrl
		{
			get { return iconUrl; }
			set { iconUrl = value; }
		}

		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		public string UrlFormat
		{
			get { return urlFormat; }
			set { urlFormat = value; }
		}

		public string[] AuthorizedRoles
		{
			get { return authorizedRoles; }
			set { authorizedRoles = value; }
		}

		public string GlobalResourceClassName
		{
			get { return globalResourceClassName; }
			set { globalResourceClassName = value; }
		}

		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		#endregion

		#region Methods

		protected abstract string ArrayVariableName { get; }

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

		public virtual Control AddTo(Control container)
		{
			HtmlAnchor a = CreateAnchor(container);
			container.Controls.Add(a);

			container.Page.ClientScript.RegisterArrayDeclaration(ArrayVariableName,
			                                                     string.Format("{{ linkId: \"{0}\", urlFormat: \"{1}\" }}",
			                                                                   a.ClientID,
			                                                                   UrlFormat.Replace("~/", Utility.ToAbsolute("~/"))));

			return a;
		}

		private HtmlAnchor CreateAnchor(Control container)
		{
			HtmlAnchor a = new HtmlAnchor();
			a.ID = "h" + Name;
			a.HRef = UrlFormat
				.Replace("~/", Utility.ToAbsolute("~/"))
				.Replace("{selected}", "")
				.Replace("{memory}", "")
				.Replace("{action}", "");

			a.Target = Target;
			a.Attributes["class"] = "command";
			a.Title = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? ToolTip;

			string title = Utility.GetResourceString(GlobalResourceClassName, Name + ".Title") ?? Title;
			if (string.IsNullOrEmpty(IconUrl))
				a.InnerHtml = title;
			else
				a.InnerHtml = string.Format("<img src='{0}'/>{1}", Utility.ToAbsolute(IconUrl), title);
			return a;
		}

		#endregion

		#region IComparable<EditingPlugInAttribute> Members

		public int CompareTo(EditingPlugInAttribute other)
		{
			if (SortOrder != other.SortOrder)
				return SortOrder - other.SortOrder;
			else
				return Name.CompareTo(other.Name);
		}

		#region Equals & GetHashCode Methods

		public override bool Equals(object obj)
		{
			if (obj == null || !obj.GetType().IsAssignableFrom(GetType()))
				return false;
			return Name == ((EditingPlugInAttribute) obj).Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion

		#endregion
	}
}