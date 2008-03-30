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
using N2.Edit;

namespace N2.Edit
{
	/// <summary>
	/// An attribute defining a toolbar item in edit mode.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
	[Obsolete("Namechange to ToolbarPluginAttribute")]
	public class ToolbarPlugInAttribute : ToolbarPluginAttribute
	{
		[Obsolete]
		public ToolbarPlugInAttribute()
			: base()
		{
		}

		[Obsolete]
		public ToolbarPlugInAttribute(string title, string name, string urlFormat, ToolbarArea area)
			: base(title, name, urlFormat, area)
		{
		}

		[Obsolete]
		public ToolbarPlugInAttribute(string title, string name, string urlFormat, ToolbarArea area, string target, string iconUrl, int sortOrder)
			: base(title, name, urlFormat, area, target, iconUrl, sortOrder)
		{
		} 
	}
}
