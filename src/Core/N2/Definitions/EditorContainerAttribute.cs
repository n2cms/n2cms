using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.UI;

namespace N2.Definitions
{
	/// <summary>Attribute classes assignable from the EditorContainerAttribute are responsible for defining and creating container controls on which editors can be added.</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public abstract class EditorContainerAttribute : Attribute, IEditableContainer
	{
		#region Constructor

		/// <summary>Creates a new instance of the editor container attribute.</summary>
		/// <param name="name">The name of this editor container.</param>
		/// <param name="sortOrder">The order of this container compared to other containers and editors. Editors within the container are sorted according to their sort order.</param>
		public EditorContainerAttribute(string name, int sortOrder)
		{
			this.name = name;
			this.sortOrder = sortOrder;
		}

		#endregion

		#region Fields

		private string name;
		private List<IContainable> containedNames = new List<IContainable>();
		private int sortOrder;
		private string containerName;
		private string[] authorizedRoles = null;
		private string localizationClassKey = "EditableContainers";

		#endregion

		#region Properties

		/// <summary>The name of this editor container. This is used to associate editable attributes to an editor container.</summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>Gets editors or sub-containers contained by this container.</summary>
		public List<IContainable> ContainedEditors
		{
			get { return containedNames; }
			set { containedNames = value; }
		}

		/// <summary>The order of this container compared to other containers and editors. Editors within the container are sorted according to their sort order.</summary>
		public int SortOrder
		{
			get { return sortOrder; }
			set { sortOrder = value; }
		}

		/// <summary>Gets or sets the name of a container containing this container.</summary>
		public string ContainerName
		{
			get { return containerName; }
			set { containerName = value; }
		}

		/// <summary>Gets or sets roles allowed to access this container.</summary>
		public string[] AuthorizedRoles
		{
			get { return authorizedRoles; }
			set { authorizedRoles = value; }
		}

		public string LocalizationClassKey
		{
			get { return localizationClassKey; }
			set { localizationClassKey = value; }
		}

		#endregion

		#region Methods

		/// <summary>Find out whether a user has permission to view this container.</summary>
		/// <param name="user">The user to check.</param>
		/// <returns>True if the user has the required permissions.</returns>
		public virtual bool IsAuthorized(IPrincipal user)
		{
			if (authorizedRoles == null)
				return true;
			else if (user == null)
				return false;

			foreach (string role in authorizedRoles)
				if (string.Equals(user.Identity.Name, role, StringComparison.OrdinalIgnoreCase) || user.IsInRole(role))
					return true;
			return false;
		}

		/// <summary>Adds an editor or sub-container definition to tihs container.</summary>
		/// <param name="subElement">The editor or sub-container to add.</param>
		public virtual void AddContained(IContainable subElement)
		{
			this.ContainedEditors.Add(subElement);
			this.ContainedEditors.Sort();
		}

		/// <summary>Gets editors and sub-containers in this container.</summary>
		/// <param name="user">The user to check.</param>
		/// <returns>A list of editors or containers the user is authorized to access.</returns>
		public virtual List<IContainable> GetContained(IPrincipal user)
		{
			List<IContainable> containables = new List<IContainable>();
			foreach (IContainable containable in this.ContainedEditors)
			{
				if (containable.IsAuthorized(user))
					containables.Add(containable);
			}
			return containables;
		}

		/// <summary>Gets a localized resource string from the global resource with the name denoted by <see cref="LocalizationClassKey"/>. The resource key follows the pattern <see cref="Name"/>.key where the name is the name of the detail and the key is the supplied parameter.</summary>
		/// <param name="key">A part of the resource key used for finding the localized resource.</param>
		/// <returns>A localized string if found, or null.</returns>
		protected virtual string GetLocalizedText(string key)
		{
			return Utility.GetGlobalResourceString(LocalizationClassKey, Name + "." + key);
		}

		#endregion

		#region Abstract Methods

		/// <summary>Adds the container to a parent container and returns it.</summary>
		/// <param name="container">The parent container onto which to add the container defined by this interface.</param>
		/// <returns>The newly added container.</returns>
		public abstract Control AddTo(Control container);

		#endregion

		#region IComparable x 2 Members

		public int CompareTo(IEditableContainer other)
		{
			return this.SortOrder.CompareTo(other.SortOrder);
		}

		public int CompareTo(IContainable other)
		{
			return this.SortOrder - other.SortOrder;
		}

		#endregion

		public override string ToString()
		{
			return this.name;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
				return false;
			else
				return ((EditorContainerAttribute) obj).Name == this.Name;
		}
	}
}