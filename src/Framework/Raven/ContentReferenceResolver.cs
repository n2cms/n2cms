using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using N2.Collections;
using N2.Persistence;
using Raven.Client;
using N2.Details;

namespace N2.Raven
{
	public class ContentReferenceResolver : IReferenceResolver
	{
		public const string ItemPrefix = "items/";
		public const string ChildrenSuffix = "/children";

		private RavenConnectionProvider connections;

		public ContentReferenceResolver(RavenConnectionProvider connections)
		{
			this.connections = connections;
		}

		public void AddReference(object context, string reference, object value)
		{
			Debug.WriteLine("AddReference " + context + " " + reference + " " + value);
		}

		public string GetReference(object context, object value)
		{
			Debug.WriteLine("GetReference " + context + " " + value);

			if (value is IContentItemList<ContentItem>)
				throw new NotImplementedException();
				//return ItemPrefix + (value as IContentItemList<ContentItem>).EnclosingItemID + ChildrenSuffix;
			if (value is ContentItem)
			    return ItemPrefix + (value as ContentItem).ID;
			return null;
		}

		public bool IsReferenced(object context, object value)
		{
			Debug.WriteLine("IsReferenced " + context + " " + value);

			return (value is IContentItemList<ContentItem>)
				|| (value is ContentItem);
		}

		public object ResolveReference(object context, string reference)
		{
			Debug.WriteLine("IsReference " + context + " " + reference);

			if (reference.StartsWith(ItemPrefix))
			{
				reference = reference.Substring(ItemPrefix.Length);
				if (reference.EndsWith(ChildrenSuffix))
					return new RavenContentItemList<ContentItem>(int.Parse(reference.Substring(0, reference.Length - ChildrenSuffix.Length)), connections.Session);
				return connections.Session.Load<ContentItem>(int.Parse(reference));
			}
			return null;
		}
	}
}
