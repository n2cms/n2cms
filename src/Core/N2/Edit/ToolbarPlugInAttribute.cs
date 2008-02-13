#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System;
using System.Web;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Definitions;
using System.Web.UI.WebControls;

namespace N2.Edit
{
	public class ToolbarAreaAttribute : Attribute, IContainable
	{
		private string containerName;
		private int sortOrder;
		private string name;

		public int SortOrder
		{
			get { return sortOrder; }
			set { sortOrder = value; }
		}

		public string ContainerName
		{
			get { return containerName; }
			set { containerName = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		

		public Control AddTo(Control container)
		{
			throw new NotImplementedException();
		}

		public bool IsAuthorized(IPrincipal user)
		{
			throw new NotImplementedException();
		}

		public int CompareTo(IContainable other)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// An attribute defining a toolbar item in edit mode.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
	public class ToolbarPlugInAttribute : EditingPlugInAttribute, IComparable<ToolbarPlugInAttribute>, IContainable
	{
		#region Fields
		ToolbarArea area;
		private string containerName;
		#endregion

		#region Constructors
		/// <summary>Defines a toolbar link.</summary>
		public ToolbarPlugInAttribute()
		{
		}

		/// <summary>Defines a toolbar link.</summary>
		/// <param name="title">The text displayed in the toolbar.</param>
		/// <param name="name">The name of this plugin (must be unique).</param>
		/// <param name="urlFormat">The url format for the url for this plugin where {selected} is the rewritten url of the currently selected item, {memory} is a cut or copied page url {action} is either move or copy.</param>
		/// <param name="area">The area to put the link.</param>		
		public ToolbarPlugInAttribute(string title, string name, string urlFormat, ToolbarArea area)
		{
			this.Title = title;
			this.Name = name;
			this.UrlFormat = urlFormat;
			this.Area = area;
		}

		/// <summary>Defines a toolbar link.</summary>
		/// <param name="title">The text displayed in the toolbar.</param>
		/// <param name="name">The name of this plugin (must be unique).</param>
		/// <param name="urlFormat">The url format for the url for this plugin where {selected} is the rewritten url of the currently selected item, {memory} is a cut or copied page url {action} is either move or copy.</param>
		/// <param name="area">The area to put the link.</param>		
		/// <param name="target">The target of the link.</param>	
		/// <param name="iconUrl">An url to an icon.</param>
		/// <param name="sortOrder">The order of this link</param>
		public ToolbarPlugInAttribute(string title, string name, string urlFormat, ToolbarArea area, string target, string iconUrl, int sortOrder)
			: this(title, name, urlFormat, area)
		{
			this.Target = target;
			this.IconUrl = iconUrl;
			this.SortOrder = sortOrder;
		} 
		#endregion

		#region Properties
		protected override string ArrayVariableName
		{
			get { return "toolbarPlugIns"; }
		}

		public ToolbarArea Area
		{
			get { return area; }
			set { area = value; }
		}
		#endregion

		#region IContainable Members

		public string ContainerName
		{
			get { return containerName; }
			set { containerName = value; }
		}

		public Control AddTo(Control container)
		{
			Literal l = new Literal();
			l.Text = Name;
			container.Controls.Add(l);
			return l;
		}
		#endregion

		#region IComparable<...> Members

		public int CompareTo(ToolbarPlugInAttribute other)
		{
			return base.CompareTo(other);
		}

		int IComparable<IContainable>.CompareTo(IContainable other)
		{
			return this.SortOrder - other.SortOrder;
		}

		#endregion
		
		
	}
}
