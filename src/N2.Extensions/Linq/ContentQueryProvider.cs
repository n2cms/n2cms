using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using N2.Details;

namespace N2.Linq
{
	public class ContentQueryProvider : IQueryProvider
	{
		readonly IQueryProvider queryProvider;
		public Dictionary<Expression, Expression> ReplacedExpressions = new Dictionary<Expression, Expression>();

		public ContentQueryProvider(IQueryProvider queryProvider)
		{
			this.queryProvider = queryProvider;
		}

		#region IQueryProvider Members

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			Debug.WriteLine("CreateQuery: " + expression);
			MethodCallExpression mcExpression = expression as MethodCallExpression;

			if (mcExpression != null && mcExpression.NodeType == ExpressionType.Call && mcExpression.Arguments.Count > 1 && mcExpression.Arguments[1].NodeType == ExpressionType.Quote)
			{
				UnaryExpression ue = mcExpression.Arguments[1] as UnaryExpression;
				Debug.WriteLine("ue: " + ue);

				if (ue != null && ue.Operand.NodeType == ExpressionType.Lambda)
				{
					var expressionToReplace = ue.Operand as Expression<Func<TElement, bool>>;
					var expressionBody = expressionToReplace.Body as BinaryExpression;
					
					Debug.WriteLine("replace: " + expressionToReplace);

					var itemParameter = Expression.Parameter(typeof(ContentItem), "ci");
					var detailsProperty = Expression.Property(itemParameter, "Details");
					var valuesProperty = Expression.Property(detailsProperty, "Values");
					var ofTypeMethod = typeof(Enumerable).GetMethod("OfType").GetGenericMethodDefinition().MakeGenericMethod(typeof(StringDetail));
					var ofTypeCall = Expression.Call(valuesProperty, ofTypeMethod, valuesProperty);
					var anyMethod = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Length == 2).GetGenericMethodDefinition().MakeGenericMethod(typeof(StringDetail));

					//var cdParameter = Expression.Parameter(typeof(StringDetail), "cd");
					//var stringValueProperty = Expression.Property(cdParameter, "StringValue");
					//var constant = Expression.Constant("hello");
					//var equalsExpression = Expression.Equal(stringValueProperty, constant);

					var detailExpression = GetDetailExpression<StringDetail>(expressionBody.Left as MemberExpression, expressionBody.Right);
					var anyCall = Expression.Call(ofTypeCall, anyMethod, ofTypeCall, detailExpression);
					
					var itemExpression = Expression.Lambda<Func<TElement, bool>>(anyCall, itemParameter);
					var quote = Expression.Quote(itemExpression);

					var replacement = Expression.Call(mcExpression.Object, mcExpression.Method, mcExpression.Arguments[0], quote);
					Debug.WriteLine("expr: " + expression);
					Debug.WriteLine("repl: " + replacement);
					expression = replacement;
				}
			}
			return new ContentQueryable<TElement>(this, queryProvider.CreateQuery<TElement>(expression));
		}

		static Expression<Func<T, bool>> GetDetailExpression<T>(MemberExpression nameExpression, Expression valueExpression) where T : ContentDetail
		{
			ParameterExpression detailParameter = Expression.Parameter(typeof(StringDetail), "ci");
			var nameEqual = GetPropertyComparison<T>(detailParameter, "Name", Expression.Constant(nameExpression.Member.Name));
			var valueEqual = GetPropertyComparison<T>(detailParameter, "StringValue", valueExpression);
			Expression nameAndValue = Expression<Func<T, bool>>.And(nameEqual, valueEqual);
			var nameAndValueExpression = Expression.Lambda<Func<T, bool>>(nameAndValue, detailParameter);
			return nameAndValueExpression;
		}

		static Expression GetPropertyComparison<T>(ParameterExpression parameterExpression, string propertyName, Expression right)
		{
			MemberExpression propertyAccess = Expression.Property(parameterExpression, propertyName);
			BinaryExpression binaryExpression = Expression.Equal(propertyAccess, right);
			return binaryExpression;
		}










		public IQueryable CreateQuery(Expression expression)
		{
			Debug.WriteLine("CreateQuery: " + expression);
			return queryProvider.CreateQuery(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			Debug.WriteLine("Execute: " + expression);
			return queryProvider.Execute<TResult>(expression);
		}

		public object Execute(Expression expression)
		{
			Debug.WriteLine("Execute: " + expression);
			return queryProvider.Execute(expression);
		}

		#endregion
	}

}
