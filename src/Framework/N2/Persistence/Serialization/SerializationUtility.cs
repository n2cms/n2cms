using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace N2.Persistence.Serialization
{
    public static class SerializationUtility
    {
        public static string ToBase64String(object value)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bf.Serialize(stream, value);
            byte[] array = stream.ToArray();
            return Convert.ToBase64String(array);
        }

        public static string GetTypeAndAssemblyName(Type type)
        {
            return string.Format("{0},{1}", type.AssemblyQualifiedName.Split(','));
        }

        public static string ToUniversalString(DateTime? value)
        {
            if (!value.HasValue)
                return "";
            return value.Value.ToUniversalTime().ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> values, TKey key)
        {
            TValue value;
            if (values.TryGetValue(key, out value))
                return value;
            return default(TValue);
        }

        public static string RemoveInvalidCharacters(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var invalidCharacters = new HashSet<char>(Enumerable.Range(0, 31).Select(n => (char)n).Where(c => c != '\t' && c != '\r' && c != '\n'));
            var text = new StringBuilder(value);
            for (int i = 0; i < text.Length; i++)
            {
                if (invalidCharacters.Contains(text[i]))
                    text.Remove(i, 1);
            }
            return text.ToString();
        }
        internal static ContentState RecaulculateState(ContentItem item)
        {
            if (!item.Published.HasValue)
                return ContentState.Draft;
            if (item.Published.HasValue && Utility.CurrentTime() < item.Published.Value)
                return ContentState.Waiting;
            if (item.Expires.HasValue && item.Expires.Value <= Utility.CurrentTime())
                return ContentState.Unpublished;

            return ContentState.Published;
        }
    }
}
