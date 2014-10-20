using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Web;
using System.Diagnostics;
using System.ComponentModel;

namespace N2.Persistence
{
    public class Relation<T> : Web.IJsonWriter
    {
        public static T DefaultAccessor(object id)
        {
            if (0.Equals(id))
                return default(T);
            throw new NotSupportedException("This relation has not been initialized for retrieving values.");
        }
        
        public Relation()
        {
            ValueAccessor = DefaultAccessor;
        }

        public virtual int? ID { get; set; }
        
        public bool HasValue 
        { 
            get { return ID.HasValue && ID.Value != 0; } 
        }

        public T Value 
        {
            get
            {
                return (ValueAccessor != null)
                    ? ValueAccessor(ID ?? 0)
                    : DefaultAccessor(ID ?? 0);
            }
        }

		[System.Xml.Serialization.XmlIgnore]
        public Func<object, T> ValueAccessor { get; set; }

        void IJsonWriter.Write(System.IO.TextWriter writer)
        {
            if (HasValue)
                writer.Write(ID);
            else
                writer.Write("null");
        }

	    public override string ToString()
	    {
		    return string.Format("{{ContentRelation id={0}, value={1} }}", ID, Value);
	    }
    }

    public class ContentRelationConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (typeof(ContentItem).IsAssignableFrom(destinationType))
                return true;
            if (typeof(int) == destinationType)
                return true;
            if (typeof(string) == destinationType)
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(ContentItem).IsAssignableFrom(destinationType))
                return ((ContentRelation)value).Value;
            if (typeof(int) == destinationType)
                return ((ContentRelation)value).ID ?? 0;
            if (typeof(string) == destinationType)
                return ((ContentRelation)value).HasValue ? ((ContentRelation)value).ID.ToString() : null;

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(ContentItem).IsAssignableFrom(sourceType))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is ContentItem)
                return (ContentRelation)((ContentItem)value);

            return base.ConvertFrom(context, culture, value);
        }
    }

    [TypeConverter(typeof(ContentRelationConverter))]
    [DebuggerDisplay("ContentRelation #{ID}")]
    public class ContentRelation : Relation<ContentItem>
    {
        private ContentItem initializedTarget;

        public ContentRelation()
        {
        }

        public ContentRelation(int id, Func<object, ContentItem> valueAccessor)
        {
            ID = id;
            ValueAccessor = valueAccessor;
        }

        public ContentRelation(ContentItem initializedTarget)
        {
            this.initializedTarget = initializedTarget;
            ValueAccessor = (id) => (initializedTarget != null && initializedTarget.ID.Equals(id))
                ? initializedTarget 
                : DefaultAccessor(id);
        }

        public override int? ID
        {
            get
            {
                if (initializedTarget != null)
                    return initializedTarget.ID;

                return base.ID;
            }
            set
            {
                if (initializedTarget != null && initializedTarget.ID != value)
                    initializedTarget = null;

                base.ID = value;
            }
        }

		private T GetValueValue<T>(Func<ContentItem, T> valueFactory)
		{
			if (!HasValue)
				return default(T);
			var value = Value;
			if (value == null)
				return default(T);

			return valueFactory(value);
		}

        public string Path
        {
            get { return GetValueValue(ci => ci.Path); }
        }
        public string Name
        {
            get { return GetValueValue(ci => ci.Name); }
        }
        public ContentRelation Parent 
        {
            get { return GetValueValue(ci => ci.Parent); } 
        }
        public ContentRelation VersionOf
        {
            get { return GetValueValue(ci => ci.VersionOf); }
        }
        public PathData FindPath(string remainingUrl) 
        {
			return GetValueValue(ci => ci.FindPath(remainingUrl));
        }
        public IContentItemList<ContentItem> Children 
        { 
            get { return GetValueValue(ci => ci.Children) ?? new ItemList<ContentItem>(); } 
        }

        public static implicit operator ContentItem(ContentRelation relation)
        {
            if (relation.HasValue)
                return relation.Value;
            return null;
        }

        public static implicit operator ContentRelation(ContentItem item)
        {
            if (item == null)
                return new ContentRelation();
            return new ContentRelation(item);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return !HasValue;

            var otherRealation = obj as Relation<ContentItem>;
            if (otherRealation != null)
            {
                if (!HasValue && !otherRealation.HasValue)
                    return true;
                if (HasValue && otherRealation.HasValue)
                    return Value == otherRealation.Value;
                return false;
            }
            var otherValue = obj as ContentItem;
            if (otherValue != null)
            {
                if (HasValue)
                    return Value == otherValue;
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static ContentRelation Empty 
        {
            get { return new ContentRelation(); }
        }
    }

    public class ParentRelation : ContentRelation
    {
    }
}
