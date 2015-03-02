using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace N2.Persistence.Xml
{
	public class ContentDataContractSurrogate : IDataContractSurrogate
	{
		public object GetCustomDataToExport(Type clrType, Type dataContractType)
		{
			return null;
		}

		public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
		{
			return null;
		}

		public Type GetDataContractType(Type type)
		{
			if (type.IsSubclassOf(typeof(MulticastDelegate)))
				return null;
			return type;
		}

		public object GetDeserializedObject(object obj, Type targetType)
		{
			return obj;
		}

		public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
		{
		}

		public object GetObjectToSerialize(object obj, Type targetType)
		{
			if (targetType.IsSubclassOf(typeof(MulticastDelegate)))
				return null;
			return obj;
		}

		public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
		{
			return null;
		}

		public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
		{
			return null;
		}
	}
}
