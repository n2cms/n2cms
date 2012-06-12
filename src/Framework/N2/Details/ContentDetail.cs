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
using N2.Collections;

namespace N2.Details
{
    /// <summary>
    /// A content detail. A number of content details can be associated with one content item.
    /// </summary>
    /// <remarks>Usually content details are created below the hood when working with primitive .NET types against a contnet item.</remarks>
	[Serializable]
	public class ContentDetail: ICloneable, INameable, IMultipleValue
	{
		#region TypeKeys
		public static class TypeKeys
		{
			public const string BoolType = "Bool";
			public const string IntType = "Int";
			public const string LinkType = "Link";
			public const string DoubleType = "Double";
			public const string DateTimeType = "DateTime";
			public const string StringType = "String";
			public const string ObjectType = "Object";
			public const string MultiType = "Multi";
		}
		#endregion

		#region Constuctors
		/// <summary>Creates a new (empty) instance of the content detail.</summary>
		public ContentDetail()
		{
			id = 0;
            enclosingItem = null;
			name = string.Empty;
			ValueTypeKey = TypeKeys.StringType;
			Value = null;
		}

		public ContentDetail(ContentItem enclosingItem, string name, object value)
		{
			ID = 0;
			EnclosingItem = enclosingItem;
			Name = name;
			Value = value;
		}
		#endregion

		#region Private Fields
		private int id;
        private ContentItem enclosingItem; 
		private string name;
		private string meta;

		private DetailCollection collection;
		private string valueTypeKey;

		private string stringValue;
		private ContentItem linkedItem;
		private int? linkValue;
		private double? doubleValue;
		private DateTime? dateTimeValue;
		private int? intValue;
		private bool? boolValue;
		private object objectValue;
		#endregion

		#region Public Properties

        /// <summary>Gets or sets the detil's primary key.</summary>
		public virtual int ID
		{
			get { return id; }
			set { id = value; }

		}

        /// <summary>Gets or sets the name of the detail.</summary>
		public virtual string Name
		{
			get { return name; }
			set	{ name = value; }
		}

		/// <summary>Meta-data used by N2 to store information about the data stored in the content detail.</summary>
		public virtual string Meta
		{
			get { return meta; }
			set { meta = value; }
		}

        /// <summary>Gets or sets this details' value.</summary>
        public virtual object Value
        {
			get
			{
				switch (ValueTypeKey)
				{
					case TypeKeys.BoolType:
						return boolValue;
					case TypeKeys.DateTimeType:
						return dateTimeValue;
					case TypeKeys.DoubleType:
						return doubleValue;
					case TypeKeys.IntType:
						return intValue;
					case TypeKeys.LinkType:
						return linkedItem;
					case TypeKeys.StringType:
						return stringValue;
					case TypeKeys.MultiType:
						return new MultipleValueHolder { BoolValue = BoolValue, DateTimeValue = DateTimeValue, DoubleValue = DoubleValue, IntValue = IntValue, LinkedItem = LinkedItem, ObjectValue = ObjectValue, StringValue = StringValue };
					default:
						return objectValue;
				}
			}
			set
			{
				valueTypeKey = SetValue(value);
			}
        }

		#region class MultipleValueHolder
		class MultipleValueHolder : IMultipleValue
		{
			#region IMultipleValue Members
			public bool? BoolValue { get; set; }
			public DateTime? DateTimeValue { get; set; }
			public double? DoubleValue { get; set; }
			public int? IntValue { get; set; }
			public ContentItem LinkedItem { get; set; }
			public object ObjectValue { get; set; }
			public string StringValue { get; set; }
			#endregion
		}
		#endregion

		private string SetValue(object value)
		{
			if (value == null)
			{
				EmptyValue();
				return valueTypeKey;
			}

			Type t = value.GetType();
			EmptyValue();
			switch (t.FullName)
			{
				case "System.Boolean":
					boolValue = (bool)value;
					return TypeKeys.BoolType;
				case "System.Int32":
					intValue = (int)value;
					return TypeKeys.IntType;
				case "System.Double":
					doubleValue = (double)value;
					return TypeKeys.DoubleType;
				case "System.DateTime":
					dateTimeValue = (DateTime)value;
					return TypeKeys.DateTimeType;
				case "System.String":
					stringValue = (string)value;
					return TypeKeys.StringType;
				default:
					if (t.IsSubclassOf(typeof(ContentItem)))
					{
						LinkedItem = (ContentItem)value;
						return TypeKeys.LinkType;
					}
					else
					{
						objectValue = value;
						return TypeKeys.ObjectType;
					}
			}
		}

		private void EmptyValue()
		{
			switch (ValueTypeKey)
			{
				case TypeKeys.BoolType:
					boolValue = false;
					return;
				case TypeKeys.DateTimeType:
					dateTimeValue = DateTime.MinValue;
					return;
				case TypeKeys.DoubleType:
					doubleValue = 0;
					return;
				case TypeKeys.IntType:
					intValue = 0;
					return;
				case TypeKeys.LinkType:
					linkedItem = null;
					return;
				case TypeKeys.StringType:
					stringValue = null;
					return;
				default:
					objectValue = null;
					return;
			}
		}

		/// <summary>Gets the type of value associated with this item.</summary>
		public virtual Type ValueType
		{
			get 
			{
				switch (ValueTypeKey)
				{
					case TypeKeys.BoolType:
						return typeof(bool);
					case TypeKeys.DateTimeType:
						return typeof(DateTime);
					case TypeKeys.DoubleType:
						return typeof(double);
					case TypeKeys.IntType:
						return typeof(int);
					case TypeKeys.StringType:
						return typeof(string);
					case TypeKeys.LinkType:
						return typeof(ContentItem);
					case TypeKeys.MultiType:
						return typeof(IMultipleValue);
					default:
						return typeof(object);
				}
			}
		}

		public virtual string ValueTypeKey
		{
			get { return valueTypeKey; }
			set { valueTypeKey = value; }
		}

		public virtual string StringValue
		{
			get { return stringValue; }
			set { stringValue = value; }
		}

		public virtual ContentItem LinkedItem
		{
			get { return linkedItem; }
			set
			{
				linkedItem = value;
				if (value != null)
					LinkValue = value.ID;
				else
					LinkValue = null;
			}
		}

		protected internal virtual int? LinkValue
		{
			get { return linkValue; }
			set { linkValue = value; }
		}

		public virtual double? DoubleValue
		{
			get { return doubleValue; }
			set { doubleValue = value; }
		}

		public virtual DateTime? DateTimeValue
		{
			get { return dateTimeValue; }
			set { dateTimeValue = value; }
		}

		public virtual bool? BoolValue
		{
			get { return boolValue; }
			set { boolValue = value; }
		}

		public virtual int? IntValue
		{
			get { return intValue; }
			set { intValue = value; }
		}

		public virtual object ObjectValue
		{
			get { return objectValue; }
			set { objectValue = value; }
		}

		/// <summary>Gets whether this items belongs to an <see cref="N2.Details.DetailCollection"/>.</summary>
		public virtual bool IsInCollection
		{
			get { return EnclosingCollection != null; }
		}

		/// <summary>Gets or sets the content item that this detail belong to.</summary>
		/// <remarks>Usually this is assigned by a content item which encapsulates the usage of details</remarks>
		public virtual N2.ContentItem EnclosingItem
		{
			get { return enclosingItem; }
			set { enclosingItem = value; }
		}

		/// <summary>Gets or sets the <see cref="N2.Details.DetailCollection"/> associated with this detail. This value can be null which means it's a named detail directly on the item.</summary>
		public virtual DetailCollection EnclosingCollection
		{
			get { return collection; }
			set { collection = value; }
		}
		#endregion 
	
		#region Static Methods
		/// <summary>Creates a new content detail of the appropriated type based on the given value.</summary>
		/// <param name="item">The item that will enclose the new detail.</param>
		/// <param name="name">The name of the detail.</param>
		/// <param name="value">The value of the detail. This will determine what type of content detail will be returned.</param>
		/// <returns>A new content detail whose type depends on the type of value.</returns>
		public static ContentDetail New(ContentItem item, string name, object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			return new ContentDetail(item, name, value);
		}

		/// <summary>Creates a new content detail of the appropriated type based on the given value.</summary>
		/// <param name="name">The name of the detail.</param>
		/// <param name="value">The value of the detail. This will determine what type of content detail will be returned.</param>
		/// <returns>A new content detail whose type depends on the type of value.</returns>
		public static ContentDetail New(string name, object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			return new ContentDetail(null, name, value);
		}

		/// <summary>Creates a new content detail with multiple values.</summary>
		/// <param name="enclosingItem">The item that will enclose the new detail.</param>
		/// <param name="name">The name of the detail.</param>
		/// <param name="booleanValue">Boolean value.</param>
		/// <param name="dateTimeValue">Date time value.</param>
		/// <param name="doubleValue">Double value.</param>
		/// <param name="integerValue">Integer value.</param>
		/// <param name="linkedValue">Linked item.</param>
		/// <param name="objectValue">Object value.</param>
		/// <param name="stringValue">String value.</param>
		/// <returns>A new content detail whose type depends on the type of value.</returns>
		public static ContentDetail Multi(string name, bool? booleanValue = null, int? integerValue = null, double? doubleValue = null, DateTime? dateTimeValue = null, string stringValue = null, ContentItem linkedValue = null, object objectValue = null)
		{
			return new ContentDetail 
			{ 
				Name = name, 
				ValueTypeKey = TypeKeys.MultiType, 
				BoolValue = booleanValue, 
				IntValue = integerValue, 
				DoubleValue = doubleValue, 
				DateTimeValue = dateTimeValue, 
				LinkedItem = linkedValue, 
				ObjectValue = objectValue,
				StringValue = stringValue
			};
		}

		/// <summary>Gets the name of the property on the detail class that can encapsulate the given value.</summary>
		/// <param name="value">The value for which the to retrieve the associated property.</param>
		/// <returns>The name of the property on the detail class that can encapsulate the given value.</returns>
		public static string GetAssociatedPropertyName(object value)
		{
			if (value is bool)
				return "BoolValue";
			else if (value is int)
				return "IntValue";
			else if (value is double)
				return "DoubleValue";
			else if (value is DateTime)
				return "DateTimeValue";
			else if (value is string)
				return "StringValue";
			else if (value is ContentItem)
				return "LinkedItem";
			else
				return "Value";
		}

		/// <summary>Gets the name of the property on the detail class that can encapsulate the given value type.</summary>
		/// <typeparam name="T">The value type for which the to retrieve the associated property.</typeparam>
		/// <returns>The name of the property on the detail class that can encapsulate the given value.</returns>
		public static string GetAssociatedPropertyName<T>()
		{
			return GetAssociatedPropertyName(typeof(T));
		}

		/// <summary>Gets the name of the property on the detail class that can encapsulate the given value type.</summary>
		/// <param name="valueType">The value type for which the to retrieve the associated property.</param>
		/// <returns>The name of the property on the detail class that can encapsulate the given value.</returns>
		public static string GetAssociatedPropertyName(Type valueType)
		{
			if (valueType == typeof(bool))
				return "BoolValue";
			else if (valueType == typeof(int))
				return "IntValue";
			else if (valueType == typeof(double))
				return "DoubleValue";
			else if (valueType == typeof(DateTime))
				return "DateTimeValue";
			else if (valueType == typeof(string))
				return "StringValue";
			else if (typeof(ContentItem).IsAssignableFrom(valueType))
				return "LinkedItem";
			else
				return "Value";
		}
		#endregion

        #region Equals, HashCode and ToString Overrides

		/// <summary>Checks details for equality.</summary>
		/// <returns>True if details have the same ID.</returns>
        public override bool Equals( object obj )
		{
			if( this == obj ) return true;
			ContentDetail other = obj as ContentDetail;
			return other != null && id != 0 && id == other.id;
		}

    	int? hashCode;
		/// <summary>Gets a hash code based on the ID.</summary>
		/// <returns>A hash code.</returns>
		public override int GetHashCode()
		{
			if (!hashCode.HasValue)
				hashCode = (id > 0 ? id.GetHashCode() : base.GetHashCode());
			return hashCode.Value;
		}

		/// <summary>Returns this details value's ToString result.</summary>
		/// <returns>The value to string.</returns>
        public override string ToString()
        {
            return null == this.Value
				? base.ToString()
				: this.Value.ToString();
        }
        #endregion

        #region ICloneable Members

        /// <summary>Creates a cloned object with the id set to 0.</summary>
        /// <returns>A new ContentDetail with the same Name and Value.</returns>
		public virtual ContentDetail Clone()
        {
			ContentDetail cloned = new ContentDetail();
            cloned.ID = 0;
            cloned.Name = this.Name;
			cloned.Meta = this.Meta;
			cloned.BoolValue = this.BoolValue;
			cloned.DateTimeValue = this.DateTimeValue;
			cloned.DoubleValue = this.DoubleValue;
			cloned.IntValue = this.IntValue;
			cloned.LinkedItem = this.LinkedItem;
			cloned.ObjectValue = this.ObjectValue;
			cloned.StringValue = this.StringValue;
			cloned.ValueTypeKey = this.ValueTypeKey;
			return cloned;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

		public virtual void AddTo(ContentItem newEnclosingItem)
		{
			AddTo((DetailCollection)null);

			if (newEnclosingItem == EnclosingItem)
				return;

			RemoveFromEnclosingItem();

			if (newEnclosingItem != null)
			{
				EnclosingItem = newEnclosingItem;
				newEnclosingItem.Details.Add(Name, this);
			}
		}

		protected internal virtual void RemoveFromEnclosingItem()
		{
			if (EnclosingItem != null)
				EnclosingItem.Details.Remove(Name);
		}

		public virtual void AddTo(DetailCollection newEnclosingCollection)
		{
			RemoveFromEnclosingCollection();

			if (newEnclosingCollection != null)
				newEnclosingCollection.Add(this);
		}

		protected internal virtual void RemoveFromEnclosingCollection()
		{
			if (EnclosingCollection != null)
				EnclosingCollection.Remove(this);
		}

		/// <summary>
		/// Copies values from the other detail onto itself.
		/// </summary>
		/// <param name="other"></param>
		public virtual void Extract(ContentDetail other)
		{
			ValueTypeKey = other.ValueTypeKey;
			Meta = meta;
			BoolValue = other.BoolValue;
			IntValue = other.intValue;
			DoubleValue = other.DoubleValue;
			DateTimeValue = other.DateTimeValue;
			LinkedItem = other.LinkedItem;
			ObjectValue = other.ObjectValue;
			StringValue = other.StringValue;
		}
	}
}
