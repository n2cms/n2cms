using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq.Functions;
using NHibernate.Linq;
using System.Reflection;

namespace N2.Linq
{
	public class WhereDetailHqlGenerator : BaseHqlGeneratorForMethod
	{
		public WhereDetailHqlGenerator(params MethodInfo[] supportedMethods)
		{
			SupportedMethods = supportedMethods;
		}

		public override NHibernate.Hql.Ast.HqlTreeNode BuildHql(System.Reflection.MethodInfo method, System.Linq.Expressions.Expression targetObject, System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> arguments, NHibernate.Hql.Ast.HqlTreeBuilder treeBuilder, NHibernate.Linq.Visitors.IHqlExpressionVisitor visitor)
		{
			throw new NotImplementedException();
		}
	}

	public class WhereDetailHqlGeneratorRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public WhereDetailHqlGeneratorRegistry()
			: base()
		{
			MethodInfo method = ReflectionHelper.GetMethodDefinition(() => QueryableExtensions.WhereDetail<ContentItem>(null, null));//.GetGenericMethodDefinition();
			RegisterGenerator(method, new WhereDetailHqlGenerator(method));
		}
	}
}
