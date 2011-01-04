using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
	}
}
