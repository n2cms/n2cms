using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Web.UI;
using N2.Definitions;

namespace N2.Edit.Settings
{
	/// <summary>
	/// Basic implementation of the <see cref="IServiceEditable"/>. This 
	/// class implements properties, provides comparison and equality but does
	/// not add any controls. Security is set to always allow.
	/// </summary>
	[Obsolete("This is too complex and will be removed, configure user controls instead.")]
	public abstract class AbstractEditableAttribute : Attribute, IServiceEditable
	{
		#region Fields
		private string title;
		private string name;
		private string serviceName;
		private string containerName = null;
		private int sortOrder; 
		#endregion

		#region Constructors
		/// <summary>Default/empty constructor.</summary>
		public AbstractEditableAttribute()
		{
		}

		/// <summary>Initializes a new instance of the AbstractEditableAttribute.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="name">The name used for equality comparison and reference.</param>
		/// <param name="sortOrder">The order of this editor</param>
		public AbstractEditableAttribute(string title, int sortOrder)
		{
			this.Title = title;
			this.SortOrder = sortOrder;
		}
		
		/// <summary>Initializes a new instance of the AbstractEditableAttribute.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="name">The name used for equality comparison and reference.</param>
		/// <param name="sortOrder">The order of this editor</param>
		public AbstractEditableAttribute(string title, string name, int sortOrder)
		{
			this.Title = title;
			this.Name = name;
			this.SortOrder = sortOrder;
		}
		#endregion

		#region IEditable Members

		/// <summary>Gets or sets the label used for presentation.</summary>
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
		/// <summary>Gets or sets the service name.</summary>
		public string ServiceName
		{
			get { return serviceName; }
			set { serviceName = value; }
		}

		public abstract void UpdateService(Engine.IEngine engine, Control editor);

		public abstract void UpdateEditor(Engine.IEngine engine, Control editor);

		#endregion

		#region IContainable Members

		/// <summary>Gets or sets the name of the detail (property) on the content item's object.</summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>Gets or sets the container name associated with this editor. The name must match a container attribute defined on the item class.</summary>
		public string ContainerName
		{
			get { return containerName; }
			set { containerName = value; }
		}

		/// <summary>Gets or sets the order of the associated control</summary>
		public int SortOrder
		{
			get { return sortOrder; }
			set { sortOrder = value; }
		}

		public abstract Control AddTo(Control container);

		/// <summary>This implementation always returns true, override it to do something useful.</summary>
		/// <param name="user">The user to check.</param>
		/// <returns>Always true.</returns>
		public virtual bool IsAuthorized(IPrincipal user)
		{
			return true;
		}

		#endregion

		#region IComparable x 2 Members

		public int CompareTo(Definitions.IContainable other)
		{
			return this.SortOrder - other.SortOrder;
		}

		/// <summary>Compares the sort order of editable attributes.</summary>
		public int CompareTo(IServiceEditable other)
		{
			if (other != null)
				return this.SortOrder - other.SortOrder;
			else
				return this.SortOrder - 0;
		}

		#endregion

		#region Equals & GetHashCode
		/// <summary>Checks another object for equality.</summary>
		/// <param name="obj">The other object to check.</param>
		/// <returns>True if the items are of the same type and have the same name.</returns>
		public override bool Equals(object obj)
		{
			IServiceEditable other = obj as IServiceEditable;
			if (other == null)
				return false;
			return Equals(other) && ServiceName == other.ServiceName;
			//return (Name == other.Name && ServiceName == other.ServiceName);
		}

		/// <summary>Gets a hash code based on the attribute's name.</summary>
		/// <returns>A hash code.</returns>
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
		#endregion
	}
}
