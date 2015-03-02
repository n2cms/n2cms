using N2.Collections;
using N2.Details;
using N2.Edit.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace N2.Persistence.Xml
{
	public class ContentDataContractResolver : DataContractResolver
	{
		public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
		{
			switch(typeName)
			{
				case "item":
					return Type.GetType(typeNamespace);
				case "items":
					return typeof(ItemList);
				case "details":
					return typeof(ContentList<ContentDetail>);
				case "collections":
					return typeof(ContentList<DetailCollection>);
				case "versions":
					return typeof(ContentVersion);
				default:
					var result = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
					return result;
			}
		}

		public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
		{
			if (typeof(ContentItem).IsAssignableFrom(type))
				return Return("item", type.Namespace + "." + type.Name + "," + type.Assembly.GetName().Name, out typeName, out typeNamespace);
			if (typeof(IEnumerable<ContentItem>).IsAssignableFrom(type))
				return Return("items", "N2", out typeName, out typeNamespace);
			if (typeof(IContentList<ContentDetail>).IsAssignableFrom(type))
				return Return("details", "N2", out typeName, out typeNamespace);
			if (typeof(IContentList<DetailCollection>).IsAssignableFrom(type))
				return Return("collections", "N2", out typeName, out typeNamespace);
			if (typeof(ContentVersion) == type)
				return Return("versions", "N2", out typeName, out typeNamespace);

			var result = knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);
			return result;
		}

		private bool Return(string name, string @namespace, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
		{
			typeName = new XmlDictionaryString(XmlDictionary.Empty, name, 0);
			typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, @namespace, 0);
			return true;
		}
	}
}
