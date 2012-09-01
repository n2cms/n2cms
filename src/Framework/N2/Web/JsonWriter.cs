using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Web.Routing;
using System.Globalization;
using System.Collections.Specialized;

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

        public JsonWriter(TextWriter sw)
        {
            this.writer = sw;
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
			else if (TryWriteArray(value as IEnumerable))
				return;
			else if (TryWriteType(value as Type))
				return;
			else if (TryWriteObject(value))
				return;
        }

        static DateTime beginningOfTime = new DateTime(1970, 01, 01);
        private bool TryWriteKnownType(object value)
        {
            switch(Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    writer.Write((bool)value ? "true" : "false");
                    return true;
                case TypeCode.DateTime:
                    {
                        var date = (DateTime)value;
                        writer.Write("\"\\/Date(" + date.Subtract(beginningOfTime).TotalMilliseconds + ")\\/\"");
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
