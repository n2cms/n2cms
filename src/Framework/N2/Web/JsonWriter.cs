using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Web.Routing;
using System.Globalization;
using System.Collections.Specialized;
using N2.Persistence;

namespace N2.Web
{
    interface IJsonWriter
    {
        void Write(TextWriter writer);
    }
    class JsonWriter
    {
        HashSet<object> visitedObjects = new HashSet<object>();
        TextWriter writer;

        public JsonWriter(TextWriter sw, bool dateCompatibility = false)
        {
            this.writer = sw;
			this.dateCompatibility = dateCompatibility;
        }

        public void Write(object value)
        {
            if (value == null)
                writer.Write("null");
            else if (TryWriteKnownType(value))
                return;
            else if (value is IJsonWriter)
                (value as IJsonWriter).Write(writer);
            else if (TryWriteDictionary(value as IDictionary<string, object>))
                return;
            else if (TryWriteDictionary(value as IDictionary))
                return;
            else if (TryWriteDictionary(value as NameValueCollection))
                return;
            else if (TryWriteArray(value as ICollection))
                return;
            else if (TryWriteType(value as Type))
                return;
            else if (TryWriteContentRelation(value as Relation<ContentItem>))
                return;
            else if (TryWriteContentItem(value as ContentItem))
                return;
            else if (TryWriteObject(value))
                return;
        }

        private bool TryWriteContentRelation(Relation<ContentItem> relation)
        {
            if (relation == null)
                return false;
            writer.Write(relation.ID);
            return true;
        }

        private bool TryWriteContentItem(ContentItem item)
        {
            if (item == null)
                return false;
            return TryWriteDictionary(item.ToDictionary());
        }

        static DateTime beginningOfTime = new DateTime(1970, 01, 01);
		private bool dateCompatibility;
        private bool TryWriteKnownType(object value)
        {
            var valueType = value.GetType();

            if (typeof(MulticastDelegate).IsAssignableFrom(valueType))
            {
                writer.Write("null");
                return true;
            }

            if (valueType.IsEnum)
            {
                writer.Write(((int)value).ToString());
                return true;
            }

            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Boolean:
                    writer.Write((bool)value ? "true" : "false");
                    return true;
                case TypeCode.DateTime:
                    {
                        var date = (DateTime)value;
						if (dateCompatibility)
							writer.Write("\"\\/Date(" + (long)date.Subtract(beginningOfTime).TotalMilliseconds + ")\\/\"");
						else
							writer.Write("\"" + (Utility.UseUniversalTime ? date : date.ToUniversalTime()).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") + "\"");
                    }
                    return true;
                case TypeCode.String:
                    return TryWriteString(value as string);
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    writer.Write(string.Format(CultureInfo.InvariantCulture, "{0}", value));
                    return true;
            }
            if (value is Uri)
            {
                writer.Write("\"" + ((Uri)value) + "\"");
                return true;
            }
            return false;
        }

        private bool TryWriteDictionary(IDictionary dictionary)
        {
            if (dictionary == null) return false;

            var first = true;
            writer.Write("{");
            foreach (var key in dictionary.Keys)
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");
                writer.Write("\"" + key + "\":");
                Write(dictionary[key]);
            }
            writer.Write("}");
            return true;
        }

        private bool TryWriteDictionary(IDictionary<string, object> dictionary)
        {
            if (dictionary == null) return false;

            var first = true;
            writer.Write("{");
            foreach (var kvp in dictionary)
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");

                writer.Write("\"" + kvp.Key + "\":");
                Write(kvp.Value);
            }
            writer.Write("}");
            return true;
        }

        private bool TryWriteDictionary(NameValueCollection dictionary)
        {
            if (dictionary == null) return false;

            var first = true;
            writer.Write("{");
            foreach (string key in dictionary.Keys)
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");
                writer.Write("\"" + key + "\":");
                Write(dictionary[key]);
            }
            writer.Write("}");
            return true;
        }

        private bool TryWriteString(string value)
        {
            if (value == null)
                return false;

            writer.Write('"');
            foreach (var ch in value)
            {
                switch (ch)
                {
                    case '\\':
                        writer.Write("\\\\");
                        continue;
                    case '\r':
                        writer.Write("\\r");
                        continue;
                    case '\n':
                        writer.Write("\\n");
                        continue;
                    case '\t':
                        writer.Write("\\t");
                        continue;
                    case '\'':
                        writer.Write("'");
                        continue;
                    case '"':
                        writer.Write("\\\"");
                        continue;
                    case ' ':
                        writer.Write(' ');
                        continue;
                    default:
                        if (ch >= 33)
                            writer.Write(ch);
                        continue;
                }
            }
            writer.Write('"');
            return true;
        }

        private bool TryWriteArray(IEnumerable list)
        {
            if (list == null) return false;

            var first = true;
            writer.Write("[");
            foreach (var item in list)
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");
                
                Write(item);
            }
            writer.Write("]");

            return true;
        }

        private bool TryWriteType(Type type)
        {
            if (type == null)
                return false;
            
            writer.Write("'" + type.AssemblyQualifiedName + "'");
            return true;
        }

        private bool TryWriteObject(object value)
        {
            if (visitedObjects.Contains(value))
            {
                writer.Write("null");
                return true;
            }
            visitedObjects.Add(value);

            return TryWriteDictionary(new RouteValueDictionary(value));
        }
    }
}
