using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace N2.RavenDB.Tests
{
	public class RavenReferenceResolver : IReferenceResolver
	{
		public void AddReference(object context, string reference, object value)
		{
			Debug.WriteLine("AddReference " + context + " " + reference + " " + value);
		}

		public string GetReference(object context, object value)
		{
			Debug.WriteLine("GetReference " + context + " " + value);
			return null;
		}

		public bool IsReferenced(object context, object value)
		{
			Debug.WriteLine("IsReference " + context + " " + value);
			return false;
		}

		public object ResolveReference(object context, string reference)
		{
			Debug.WriteLine("IsReference " + context + " " + reference);
			return null;
		}
	}
}
