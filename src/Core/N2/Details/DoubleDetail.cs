/* Copyright (C) 2006 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

using System;
using System.Collections;
using System.Diagnostics;

namespace N2.Details
{
    /// <summary>
    /// An double content detail. A number of content details can be associated 
    /// with one content item.
    /// </summary>
	[Serializable]
	[DebuggerDisplay("{Name}: DoubleValue: {DoubleValue}")]
	public class DoubleDetail : ContentDetail
	{
        #region Constuctors
		public DoubleDetail() : base()
		{
		}

		public DoubleDetail(ContentItem containerItem, string name, double value) 
		{
            this.ID = 0;
            this.EnclosingItem = containerItem;
            this.Name = name;
            this.doubleValue = value;
		}
		#endregion

        #region Properties
		private double doubleValue;

        public virtual double DoubleValue
        {
            get { return doubleValue; }
            set { doubleValue = value; }
        }

        public override object Value
        {
            get { return this.doubleValue; }
            set { this.doubleValue = (double)value; }
        }

		public override Type ValueType
		{
			get { return typeof(double); }
		}
        #endregion
    }
}
