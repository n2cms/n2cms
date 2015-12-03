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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using N2.Collections;
using N2.Persistence;

namespace N2.Details
{
    /// <summary>
    /// A content detail. A number of content details can be associated with one content item.
    /// </summary>
    /// <remarks>Usually content details are created below the hood when working with primitive .NET types against a contnet item.</remarks>
    [Serializable]
    [DebuggerDisplay("ContentDetail, {name} #{id}: {Value}")]
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
            public const string EnumType = "Enum";
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
        private ContentRelation linkedItem;
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
            set { name = value; }
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
                        return LinkedItem.Value;
                    case TypeKeys.StringType:
                        return stringValue;
                    case TypeKeys.EnumType:
                        return Enum.Parse(Type.GetType(meta, true, true), stringValue, true);
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
            public ContentRelation LinkedItem { get; set; }
            public object ObjectValue { get; set; }
            public string StringValue { get; set; }
            #endregion

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is bool)
                    return ((bool)obj).Equals(BoolValue);
                if (obj is DateTime)
                    return ((DateTime)obj).Equals(DateTimeValue);
                if (obj is double)
                    return ((double)obj).Equals(DoubleValue);
                if (obj is int)
                    return ((int)obj).Equals(IntValue);
                if (obj is ContentItem)
                    return (obj as ContentItem).Equals(LinkedItem);
                if (obj is string)
                    return (obj as string).Equals(StringValue);
                if (obj != null)
                    return obj.Equals(ObjectValue);

                return false;
            }
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

            if (t.FullName == "System.Boolean")
            {
                boolValue = (bool)value;
                return TypeKeys.BoolType;
            }
            else if (t.FullName == "System.Int32")
            {
                intValue = (int)value;
                return TypeKeys.IntType;
            }
            else if (t.FullName == "System.Double")
            {
                doubleValue = (double)value;
                return TypeKeys.DoubleType;
            }
            else if (t.FullName == "System.DateTime")
            {
                dateTimeValue = (DateTime)value;
                return TypeKeys.DateTimeType;
            }
            else if (t.FullName == "System.String")
            {
                stringValue = (string)value;
                return TypeKeys.StringType;
            }
            else if (t.IsEnum)
            {
                meta = t.AssemblyQualifiedName;
                stringValue = value.ToString();
                IntValue = (int)value;
                return TypeKeys.EnumType;
            }
            else if (t.IsSubclassOf(typeof(ContentItem)))
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
                    LinkedItem = null;
                    return;
                case TypeKeys.StringType:
                    stringValue = null;
                    return;
                case TypeKeys.EnumType:
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
                    case TypeKeys.EnumType:
                        return typeof(Enum);
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

        public virtual ContentRelation LinkedItem
        {
            get { return linkedItem ?? (linkedItem = ContentRelation.Empty); }
            set
            {
                linkedItem = value;
                if (value != null)
                    LinkValue = value.ID;
                else
                    LinkValue = null;
            }
        }

        public virtual int? LinkValue
        {
            get { return LinkedItem.ID; }
            set { LinkedItem.ID = value; }
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
        [System.Xml.Serialization.XmlIgnore]
		public virtual N2.ContentItem EnclosingItem
        {
            get { return enclosingItem; }
            set { enclosingItem = value; }
        }

        /// <summary>Gets or sets the <see cref="N2.Details.DetailCollection"/> associated with this detail. This value can be null which means it's a named detail directly on the item.</summary>
		[System.Xml.Serialization.XmlIgnore]
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
        public static ContentDetail New(ContentItem item, string name, object value, string meta = null)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new ContentDetail(item, name, value) { Meta = meta };
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

        /// <summary>Gets the associated property name for an enumerable collection used for storing In or NotIn comparison values.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetAssociatedEnumerablePropertyName(IEnumerable value)
        {
            if (value == null)
                throw new NotSupportedException();
            
            Type collectionType = value.GetType();
            if (collectionType.IsArray && collectionType.GetElementType() != typeof(object))
                return ContentDetail.GetAssociatedPropertyName(collectionType.GetElementType());
            if (collectionType.IsGenericType && collectionType.GetGenericArguments()[0] != typeof(object))
                return ContentDetail.GetAssociatedPropertyName(collectionType.GetGenericArguments()[0]);

            return ContentDetail.GetAssociatedPropertyName(value.OfType<object>().FirstOrDefault());
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
                return "LinkedItem.ID";
            else if (value is Enum)
                return "StringValue";
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
            else if (typeof(Enum).IsAssignableFrom(valueType))
                return "EnumValue";
            else if (typeof(ContentItem).IsAssignableFrom(valueType))
                return "StringValue";
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

            if (ObjectValue == null)
                cloned.ObjectValue = null;
            else
            {
                if (ObjectValue is ICloneable)
                    cloned.objectValue = (ObjectValue as ICloneable).Clone();
                else if (ObjectValue.GetType().IsValueType)
                    cloned.objectValue = ObjectValue;
                else if (ObjectValue.GetType().IsSerializable)
                    cloned.objectValue = ObjectCopier.Clone(ObjectValue);
                else
                    throw new InvalidDataException("Detail cannot be cloned: " + Name + " - " + ObjectValue.GetType().FullName);
            }

            cloned.StringValue = this.StringValue;
            cloned.ValueTypeKey = this.ValueTypeKey;
            return cloned;
        }

        public virtual ContentDetail Clone(string newPartName)
        {
            var detail = this.Clone();
            detail.name = newPartName;
            return detail;
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
            if (EnclosingItem != null && EnclosingItem.Details.Contains(this))
                EnclosingItem.Details.Remove(this);
        }

        public virtual void AddTo(DetailCollection newEnclosingCollection)
        {
            RemoveFromEnclosingCollection();

            if (newEnclosingCollection != null)
                newEnclosingCollection.Add(this);
        }

        protected internal virtual void RemoveFromEnclosingCollection()
        {
            if (EnclosingCollection != null && EnclosingCollection.Contains(this))
                EnclosingCollection.Remove(this);
        }

        /// <summary>
        /// Copies values from the other detail onto itself.
        /// </summary>
        /// <param name="other"></param>
        public virtual void Extract(ContentDetail other)
        {
            ValueTypeKey = other.ValueTypeKey;
            Meta =  other.Meta;
            BoolValue = other.BoolValue;
            IntValue = other.intValue;
            DoubleValue = other.DoubleValue;
            DateTimeValue = other.DateTimeValue;
            LinkedItem = other.LinkedItem;
            ObjectValue = other.ObjectValue;
            StringValue = other.StringValue;
        }

        public static object ExtractQueryValue(object value)
        {
            if (value is Enum)
                return value.ToString();
            if (value is ContentItem)
                return ((ContentItem)value).ID;

            return value;
        }
    }


    /// <summary>
    /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
    /// Provides a method for performing a deep copy of an object.
    /// Binary Serialization is used to perform the copy.
    /// </summary>
    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
}

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
