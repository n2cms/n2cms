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

namespace N2.Details
{
    /// <summary>Base class used to associate a content item's detail/property with a control that will handle editing or displaying.</summary>
    public abstract class ControlAssociationAttribute : System.Attribute, IComparable<ControlAssociationAttribute>
    {
        #region Private Members
        private Type controlType;
        private string controlPropertyName;
        private string title;
        private string name;
        private int sortOrder;
        private bool dataBind = false;
        private bool focus = false;
        #endregion

        #region Properties
        /// <summary>Gets or sets whether the control should be databound when it's added to a page.</summary>
        public bool DataBind
        {
            get { return dataBind; }
            set { dataBind = value; }
        }

        /// <summary>Gets or sets whether the control should be focused when it's added to a page.</summary>
        public bool Focus
        {
            get { return focus; }
            set { focus = value; }
        }

        /// <summary>Gets or sets the name of the detail (property) on the content item's object.</summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>Gets or sets the label used for presentation.</summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>Gets or sets the type of the control that is used in combination with an item's property/detail.</summary>
        public Type ControlType
        {
            get { return controlType; }
            set { controlType = value; }
        }

        /// <summary>Gets or sets the property on the control that is used to get or set content data.</summary>
        public string ControlPropertyName
        {
            get { return controlPropertyName; }
            set { controlPropertyName = value; }
        }

        /// <summary>Gets or sets the order of the associated control</summary>
        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
        #endregion

        #region Equals & GetHashCode
        /// <summary>Checks another object for equality.</summary>
        /// <param name="obj">The other object to check.</param>
        /// <returns>True if the items are of the same type and have the same name.</returns>
        public override bool Equals(object obj)
        {
            ControlAssociationAttribute other = obj as ControlAssociationAttribute;
            if (other == null)
                return false;
            return (this.Name == other.Name);
        }

        /// <summary>Gets a hash code based on the attribute's name.</summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        #endregion

        #region IComparable<ControlAssociationAttribute> Members

        int IComparable<ControlAssociationAttribute>.CompareTo(ControlAssociationAttribute other)
        {
            return this.sortOrder - other.sortOrder;
        }

        #endregion
    }
}
