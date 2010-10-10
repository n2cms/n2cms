using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace N2.Persistence.Proxying
{
	internal static class MethodInfoExtensions
	{
		public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType)
		{
			return memberInfo.GetCustomAttributes(attributeType, false).Length > 0;
		}
		public static bool IsCompilerGenerated(this MethodInfo method)
		{
			return method.HasAttribute(typeof(CompilerGeneratedAttribute));
		}
		public static bool IsCompilerGenerated(this PropertyInfo property)
		{
			return (property.GetGetMethod() ?? property.GetSetMethod()).IsCompilerGenerated();
		}
	}

}
