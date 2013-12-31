using System;
using System.Collections;

namespace N2.Engine.Globalization
{
    public class DictionaryScope : IDisposable
    {
        IDictionary dictionary;
        object previousValue = null;
        object key;

        public DictionaryScope(IDictionary dictionary, object key, object value)
        {
            if (dictionary.Contains(key))
                previousValue = dictionary[key];
            dictionary[key] = value;
            this.key = key;
            this.dictionary = dictionary;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (previousValue == null)
                dictionary.Remove(key);
            else
                dictionary[key] = previousValue;
        }

        #endregion
    }
}
