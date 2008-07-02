using System;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using N2.Edit.Web;
using N2.Plugin;

namespace N2.Edit
{
	/// <summary>
	/// Classes extending this abstract class are collected and may be 
	/// retrieved by user interfaces in the editing interface.
	/// </summary>
	public abstract class AdministrativePluginAttribute : Attribute, IComparable<IPlugin>, IPlugin
	{
		private string[] authorizedRoles;
		private bool enabled = true;
		private string name;
		private int sortOrder = int.MaxValue;

		#region Public Properties

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

		public abstract Control AddTo(Control container);

		protected string GetSelectedPath(Control container)
		{
			if (container.Page is EditPage)
			{
				EditPage ep = container.Page as EditPage;
				return HttpUtility.UrlEncode(ep.SelectedItem.Path);
			}
			return HttpUtility.UrlEncode("/");
        }

        #region IComparable<IPlugin> Members

        public int CompareTo(IPlugin other)
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