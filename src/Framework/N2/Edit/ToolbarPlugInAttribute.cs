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

namespace N2.Edit
{
    /// <summary>
    /// An attribute defining a toolbar item in edit mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public class ToolbarPluginAttribute : LinkPluginAttribute
    {
        ToolbarArea area;
        private string containerName;

        /// <summary>Defines a toolbar link.</summary>
        public ToolbarPluginAttribute()
        {
        }

        /// <summary>Defines a toolbar link.</summary>
        /// <param name="title">The text displayed in the toolbar.</param>
        /// <param name="name">The name of this plugin (must be unique).</param>
        /// <param name="urlFormat">The url format for the url for this plugin where {selected} is the rewritten url of the currently selected item, {memory} is a cut or copied page url {action} is either move or copy.</param>
        /// <param name="area">The area to put the link.</param>        
        public ToolbarPluginAttribute(string title, string name, string urlFormat, ToolbarArea area)
        {
            Title = title;
            Name = name;
            UrlFormat = urlFormat;
            Area = area;
        }

        /// <summary>Defines a toolbar link.</summary>
        /// <param name="title">The text displayed in the toolbar.</param>
        /// <param name="name">The name of this plugin (must be unique).</param>
        /// <param name="urlFormat">The url format for the url for this plugin where {selected} is the rewritten url of the currently selected item, {memory} is a cut or copied page url {action} is either move or copy.</param>
        /// <param name="area">The area to put the link.</param>        
        /// <param name="target">The target of the link.</param>    
        /// <param name="iconUrl">An url to an icon.</param>
        /// <param name="sortOrder">The order of this link</param>
        public ToolbarPluginAttribute(string title, string name, string urlFormat, ToolbarArea area, string target, string iconUrl, int sortOrder)
            : this(title, name, urlFormat, area)
        {
            Target = target;
            IconUrl = iconUrl;
            SortOrder = sortOrder;
        } 

        public ToolbarArea Area
        {
            get { return area; }
            set { area = value; }
        }
        
        public string ContainerName
        {
            get { return containerName; }
            set { containerName = value; }
        }

        /// <summary>
        /// A type implementing N2.Engine.IProvider(Of ToolbarOption)
        /// </summary>
        public Type OptionProvider { get; set; }
    }
}
