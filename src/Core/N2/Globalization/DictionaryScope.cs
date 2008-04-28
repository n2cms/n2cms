using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace N2.Globalization
{
	public class DictionaryScope : Scope
	{
		object previousValue = null;

		public DictionaryScope(IDictionary dictionary, object key, object value)
			: base(delegate
			{
				if (dictionary.Contains(key))
					previousValue = dictionary[key];
				dictionary[key] = value;
			}, delegate
			{
				if (previousValue == null)
					dictionary.Remove(key);
				else
					dictionary[key] = previousValue;
			})
		{
		}
	}
}
