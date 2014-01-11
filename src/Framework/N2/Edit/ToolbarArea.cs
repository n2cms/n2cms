#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using System;

namespace N2.Edit
{
    /// <summary>An area in which to place edit mode plugins.</summary>
    [Flags]
    public enum ToolbarArea
    {
        None = 0,
        /// <summary>The far left area above the navigation area.</summary>
        Navigation = 1,
        /// <summary>The far right area above the preview area.</summary>
        Preview = 2,
        /// <summary>Both Navigation and Preview panes. Used when referencing multiple panes in user interfaces.</summary>
        Both = 3,
        /// <summary>The central area above the preview area.</summary>
        Operations = 4,
        /// <summary>Before the search box, above the tool bar.</summary>
        Options = 8,
        /// <summary>Displayed across all management interfaces.</summary>
        Management = 16,
        /// <summary>Displayed on files.</summary>
        Files = 32
    }
}
