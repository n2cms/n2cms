using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using N2.Persistence.Finder;

namespace N2.Linq
{
	public class ContentQuery<T> : IQueryable<T>
	{
		public ContentQuery(ContentQueryProvider queryProvider)
		{
			Provider = queryProvider;
			ElementType = typeof(T);
			Expression = Expression<T>.Constant(this);
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)Provider.Execute(Expression)).GetEnumerator();
		}

		#endregion

		#region IQueryable Members

		public Type ElementType { get; set; }

		public Expression Expression { get; set; }

		public IQueryProvider Provider { get; set; }

		#endregion
	}
	public class ContentQueryProvider : IQueryProvider
	{
		readonly IItemFinder finder;

		public ContentQueryProvider(IItemFinder finder)
		{
			this.finder = finder;
		}

		#region IQueryProvider Members

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new ContentQuery<TElement>(this) { Expression = expression };
		}

		public IQueryable CreateQuery(Expression expression)
		{
			throw new NotImplementedException();
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult)Execute(expression);
		}

		public object Execute(Expression expression)
		{
			LambdaUtilities.PrintUnkwnown(expression);
			Type t = TypeSystem.GetElementType(expression.Type);
			ExpressionParser parser = new ExpressionParser(finder.Where.Type.Eq(t));
			return parser.Execute(expression);

			//IQueryBuilder builder = finder.Where;

			//if(expression.NodeType == ExpressionType.Call)
			//{
			//    MethodCallExpression mce = expression as MethodCallExpression;


			//    ConstantExpression first = mce.Arguments[0] as ConstantExpression;
			//    if(mce.Method.Name == "Where")
			//    {
			//        UnaryExpression second = mce.Arguments[1] as UnaryExpression;
			//        AppendQuery(second.Operand, builder);
			//        return Finalize(second.Method.Name, first, builder);
			//    }

			//    return Finalize(mce.Method.Name, first, builder);

			//    var method = mce.Method;
			//    var args = mce.Arguments;
			//    var o = mce.Object;
			//    Debug.WriteLine(mce.ToString());
			//}

			//if(expression.NodeType == ExpressionType.Constant)
			//{
			//    return Finalize("", expression as ConstantExpression, builder);
			//}

			//throw new NotImplementedException();
		}

		private void AppendQuery(Expression expression, IQueryBuilder builder)
		{

		}

		object Finalize(string methodName, ConstantExpression ce, IQueryBuilder builder)
		{
			Type itemType = ((IQueryable)ce.Value).ElementType;
			IQueryAction action = builder.Type.Eq(itemType);
			if (methodName == "Count")
				return action.Count();

			MethodInfo method = action.GetType().GetMethods().Where(m => m.Name == "Select" && m.IsGenericMethod).First();
			return method.MakeGenericMethod(itemType).Invoke(action, null);
		}

		#endregion
	}

	public class ExpressionParser
	{
		readonly IQueryAction builder;

		public ExpressionParser(IQueryAction builder)
		{
			this.builder = builder;
		}

		public object Execute(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Call)
			{
				return new CallExpressionParser(builder).Execute(expression as MethodCallExpression);
			}
			throw new NotImplementedException();
		}
	}


	internal class CallExpressionParser
	{
		readonly IQueryAction builder;

		public CallExpressionParser(IQueryAction builder)
		{
			this.builder = builder;
		}

		public object Execute(MethodCallExpression expression)
		{
			if (expression.Method.Name == "Select")
				return builder.Select();
			throw new NotImplementedException();
		}
	}


	public class LambdaUtilities
	{
		public static int PrintUnkwnown(Expression expression)
		{
			if (expression == null)
				return Write("(null)");

			if (expression is LambdaExpression)
				return Print(expression as LambdaExpression);
			if (expression is ParameterExpression)
				return PrintParameter(expression as ParameterExpression);
			if (expression is MemberExpression)
				return PrintMember(expression as MemberExpression);
			if (expression is BinaryExpression)
				return PrintBinary(expression as BinaryExpression);
			if (expression is ConstantExpression)
				return PrintConstant(expression as ConstantExpression);
			if (expression is MethodCallExpression)
				return PrintMethodCall(expression as MethodCallExpression);
			throw new NotImplementedException("Not implemented for " + expression.GetType());
		}

		private static int PrintMethodCall(MethodCallExpression expression)
		{
			Write("ToString: " + expression);
			Write("GetType: " + expression.GetType());
			Write("NodeType: " + expression.NodeType);
			Write("Type: " + expression.Type);
			foreach (var argument in expression.Arguments)
			{
				Write("Argument: " + argument);
			}
			Write("Method.Name: " + expression.Method.Name);
			Write("Object: " + expression.Object);
			PrintUnkwnown(expression.Object);

			return 0;
		}

		private static int PrintConstant(ConstantExpression expression)
		{
			Write("ToString: " + expression);
			Write("GetType: " + expression.GetType());
			Write("NodeType: " + expression.NodeType);
			Write("Type: " + expression.Type);
			Write("Value: " + expression.Value);
			return 0;
		}

		private static int Print(LambdaExpression expression)
		{
			Write("ToString: " + expression);
			Write("GetType: " + expression.GetType());
			Write("Body: " + expression.Body);
			PrintUnkwnown(expression.Body);
			Write("NodeType: " + expression.NodeType);
			Write("Parameters: " + expression.Parameters);
			Write("Parameters.Count: " + expression.Parameters.Count);
			for (int i = 0; i < expression.Parameters.Count; i++)
			{
				Write("Parameters[" + i + "]: " + expression.Parameters[i]);
				PrintUnkwnown(expression.Parameters[i]);
			}
			Write("Type: " + expression.Type);
			return 0;
		}

		private static int PrintMember(MemberExpression expression)
		{
			Write("ToString: " + expression);
			Write("GetType: " + expression.GetType());
			Write("Expression: " + expression.Expression);
			PrintUnkwnown(expression.Expression);
			Write("Member: " + expression.Member);
			Write("NodeType: " + expression.NodeType);
			Write("Type: " + expression.Type);
			return 0;
		}

		private static int PrintBinary(BinaryExpression expression)
		{
			Write("ToString: " + expression);
			Write("GetType: " + expression.GetType());
			Write("Conversion: " + expression.Conversion);
			Write("IsLifted: " + expression.IsLifted);
			Write("IsLiftedToNull: " + expression.IsLiftedToNull);
			Write("Left: " + expression.Left);
			PrintUnkwnown(expression.Left);
			Write("Method: " + expression.Method);
			Write("NodeType: " + expression.NodeType);
			Write("Right: " + expression.Right);
			PrintUnkwnown(expression.Right);
			Write("Type: " + expression.Type);
			return 0;
		}

		private static int PrintParameter(ParameterExpression expression)
		{
			Write("ToString: " + expression);
			Write("GetType: " + expression.GetType());
			Write("Name: " + expression.Name);
			Write("NodeType: " + expression.NodeType);
			Write("Type: " + expression.Type);
			return 0;
		}

		private static int Write(string text)
		{
			int length = new StackTrace().GetFrames().Length;
			for (int i = 15; i < length; i++)
			{
				Console.Write(' ');
			}
			Console.WriteLine(text);
			return 0;
		}
	}

	internal static class TypeSystem
	{
		internal static Type GetElementType(Type seqType)
		{
			Type ienum = FindIEnumerable(seqType);
			if (ienum == null) return seqType;
			return ienum.GetGenericArguments()[0];
		}

		private static Type FindIEnumerable(Type seqType)
		{
			if (seqType == null || seqType == typeof(string))
				return null;

			if (seqType.IsArray)
				return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

			if (seqType.IsGenericType)
			{
				foreach (Type arg in seqType.GetGenericArguments())
				{
					Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
					if (ienum.IsAssignableFrom(seqType))
					{
						return ienum;
					}
				}
			}

			Type[] ifaces = seqType.GetInterfaces();
			if (ifaces != null && ifaces.Length > 0)
			{
				foreach (Type iface in ifaces)
				{
					Type ienum = FindIEnumerable(iface);
					if (ienum != null) return ienum;
				}
			}

			if (seqType.BaseType != null && seqType.BaseType != typeof(object))
			{
				return FindIEnumerable(seqType.BaseType);
			}

			return null;

		}

	}
	public static class Find<T> where T : ContentItem
	{
		public static IQueryable<T> Items
		{
			get { return new ContentQuery<T>(new ContentQueryProvider(Find.Items)); }
		}
	}
}