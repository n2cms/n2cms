using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Definitions;
using System.Diagnostics;

namespace N2.Details
{
	[DebuggerDisplay("{name, nq} [{TypeName, nq}]")]
	public abstract class AbstractDisplayableAttribute : Attribute, IDisplayable
	{
		private string cssClass = null;
		private string name;
		int? hashCode;

		public string CssClass
		{
			get { return cssClass; }
			set { cssClass = value; }
		}

		#region IDisplayable Members
		/// <summary>Gets or sets the name of the detail (property) on the content item's object.</summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public abstract Control AddTo(ContentItem item, string detailName, Control container);
		#endregion

		#region Equals & GetHashCode
		/// <summary>Checks another object for equality.</summary>
		/// <param name="obj">The other object to check.</param>
		/// <returns>True if the items are of the same type and have the same name.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as AbstractDisplayableAttribute;
			if (other == null)
				return false;
			return name == other.Name;
		}

		/// <summary>Gets a hash code based on the attribute's name.</summary>
		/// <returns>A hash code.</returns>
		public override int GetHashCode()
		{
			return hashCode ?? (hashCode = (name == null) ? base.GetHashCode() : (GetType().FullName.GetHashCode() + name.GetHashCode())).Value;
		}

		private string TypeName
		{
			get { return GetType().Name; }
		}
		#endregion
	}
}
