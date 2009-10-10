using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using N2.Details;

namespace N2.Linq
{
	/// <summary>
	/// Translates queries against properties on an object to detail sub-select queries
	/// </summary>
	public class ContentQueryProvider : IQueryProvider
	{
		static MethodInfo anyMethodInfo = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Length == 2).GetGenericMethodDefinition();
		static MethodInfo ofTypeMethodInfo = typeof(Enumerable).GetMethod("OfType").GetGenericMethodDefinition();
			
		readonly IQueryProvider queryProvider;
		internal Dictionary<Expression, bool> WhereDetailExpressions = new Dictionary<Expression, bool>();
		
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
				
				if (ue != null && ue.Operand.NodeType == ExpressionType.Lambda)
				{
					var expressionToReplace = ue.Operand as Expression<Func<TElement, bool>>; // ci => (ci.StringProperty2 = "another string")

					if (expressionToReplace != null && WhereDetailExpressions.ContainsKey(expressionToReplace) && expressionToReplace.Body is BinaryExpression)
					{
						var expressionBody = expressionToReplace.Body as BinaryExpression; // (ci.StringProperty2 = "another string")
						var translatedExpression = TranslateExpression<TElement>(expressionBody);
						Expression quote = Expression.Quote(translatedExpression);
						expression = Expression.Call(mcExpression.Object, mcExpression.Method, mcExpression.Arguments[0], quote); // value(NHibernate.Linq.Query`1[N2.Extensions.Tests.Linq.LinqItem]).Where(ci => ci.Details.Values.OfType().Any(cd => ((cd.Name = "StringProperty2") && (cd.StringValue = "another string"))))
					}
				}
			}

			return new ContentQueryable<TElement>(this, queryProvider.CreateQuery<TElement>(expression));
		}

		Expression TranslateExpression<TElement>(BinaryExpression expressionBody)
		{
			switch (expressionBody.NodeType)
			{
				case ExpressionType.Equal:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.NotEqual:
					return TranslatePropertyComparisonExpression<TElement>(expressionBody);
				//case ExpressionType.AndAlso:
				//    return TranslateAndAlsoExpression<TElement>(expressionBody);
				default:
					throw new NotSupportedException("Not supported expression type " + expressionBody.NodeType + " in expression " + expressionBody);
			}
		}

		Expression TranslateAndAlsoExpression<TElement>(BinaryExpression expressionBody)
		{
			var left = TranslateExpression<TElement>(expressionBody.Left as BinaryExpression);
			var right = TranslateExpression<TElement>(expressionBody.Right as BinaryExpression);
			return Expression.AndAlso(left, right);
		}

		Expression TranslatePropertyComparisonExpression<TElement>(BinaryExpression expressionBody)
		{
			ComparisonInfo comparison = ComparisonInfo.Get(expressionBody);

			Type propertyType = GetExpressionType(comparison.NameExpression);
			var detail = GetDetailFromPropertyType(propertyType);

			var itemParameter = Expression.Parameter(typeof(ContentItem), "ci"); // ci
			var valuesProperty = Expression.Property(Expression.Property(itemParameter, "Details"), "Values"); // ci.Details.Values
			var ofTypeMethod = ofTypeMethodInfo.MakeGenericMethod(detail.DetailType); // System.Collections.Generic.IEnumerable`1[N2.Details.StringDetail] OfType[StringDetail](System.Collections.IEnumerable)
			var ofTypeCall = Expression.Call(valuesProperty, ofTypeMethod, valuesProperty); // ci.Details.Values.OfType()
			var anyMethod = anyMethodInfo.MakeGenericMethod(detail.DetailType); // Boolean Any[StringDetail](System.Collections.Generic.IEnumerable`1[N2.Details.StringDetail], System.Func`2[N2.Details.StringDetail,System.Boolean])

			var detailExpression = GetDetailExpression(comparison, detail); // cd => ((cd.Name = "StringProperty2") && (cd.StringValue = "another string"))
			var anyCall = Expression.Call(ofTypeCall, anyMethod, ofTypeCall, detailExpression); // ci.Details.Values.OfType().Any(cd => ((cd.Name = "StringProperty2") && (cd.StringValue = "another string")))

			var itemExpression = Expression.Lambda<Func<TElement, bool>>(anyCall, itemParameter); // ci => ci.Details.Values.OfType().Any(cd => ((cd.Name = "StringProperty2") && (cd.StringValue = "another string")))
			return itemExpression;
		}

		private Type GetExpressionType(MemberExpression nameExpression)
		{
			if (nameExpression.Member.MemberType == MemberTypes.Property)
				return ((PropertyInfo) nameExpression.Member).PropertyType;

			throw new NotSupportedException("Comparison of " + nameExpression);
		}

		private DetailInfo GetDetailFromPropertyType(Type propertyType)
		{
			if (propertyType == typeof(string))
				return new DetailInfo(typeof(StringDetail), "StringValue");
			if (propertyType == typeof(bool))
				return new DetailInfo(typeof(BooleanDetail), "BoolValue");
			if (propertyType == typeof(int))
				return new DetailInfo(typeof(IntegerDetail), "IntValue");
			if (propertyType == typeof(double))
				return new DetailInfo(typeof (DoubleDetail), "DoubleValue");
			if (propertyType == typeof(DateTime))
				return new DetailInfo(typeof(DateTimeDetail), "DateTimeValue");
			if (propertyType.IsSubclassOf(typeof(ContentItem)))
				return new DetailInfo(typeof(LinkDetail), "LinkedItem");

			return new DetailInfo(typeof(ObjectDetail), "Value");
		}

		static Expression GetDetailExpression(ComparisonInfo comparison, DetailInfo detail)
		{
			ParameterExpression detailParameter = Expression.Parameter(detail.DetailType, "cd");
			var nameEqual = GetPropertyEquals(detailParameter, "Name", Expression.Constant(comparison.NameExpression.Member.Name));
			var valueExpression = GetPropertyComparison(detailParameter, detail.ValuePropertyName, comparison);
			Expression nameAndValue = Expression.AndAlso(nameEqual, valueExpression);
			var nameAndValueExpression = Expression.Lambda(nameAndValue, detailParameter);
			return nameAndValueExpression;
		}

		static Expression GetPropertyEquals(ParameterExpression parameterExpression, string propertyName, Expression valueExpression)
		{
			MemberExpression propertyAccess = Expression.Property(parameterExpression, propertyName);
			BinaryExpression binaryExpression = Expression.Equal(propertyAccess, valueExpression);
			return binaryExpression;
		}

		static Expression GetPropertyComparison(ParameterExpression parameterExpression, string propertyName, ComparisonInfo comparison)
		{
			MemberExpression propertyAccess = Expression.Property(parameterExpression, propertyName);

			Expression left;
			Expression right;
			if(comparison.IsLeftToRight)
			{
				left = propertyAccess;
				right = comparison.ValueExpression;
			}
			else
			{
				right = propertyAccess;
				left = comparison.ValueExpression;
			}

			switch (comparison.Type)
			{
				case ExpressionType.Equal:
					return Expression.Equal(left, right);
				case ExpressionType.GreaterThan:
					return Expression.GreaterThan(left, right);
				case ExpressionType.GreaterThanOrEqual:
					return Expression.GreaterThanOrEqual(left, right);
				case ExpressionType.LessThan:
					return Expression.LessThan(left, right);
				case ExpressionType.LessThanOrEqual:
					return Expression.LessThanOrEqual(left, right);
				case ExpressionType.NotEqual:
					return Expression.NotEqual(left, right);

				default:
					throw new NotSupportedException("Expression of type " + comparison.Type + " is not supported");
			}
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

		struct ComparisonInfo
		{
			public MemberExpression NameExpression;
			public Expression ValueExpression;
			public bool IsLeftToRight;
			public ExpressionType Type;

			internal static ComparisonInfo Get(BinaryExpression expressionBody)
			{
				ComparisonInfo info;

				info.NameExpression = expressionBody.Left as MemberExpression; // ci.IntProperty
				info.ValueExpression = expressionBody.Right; // 123
				info.IsLeftToRight = true;
				info.Type = expressionBody.NodeType;
				
				if (info.NameExpression == null)
				{
					// to support ("constant" == item.Property)
					info.NameExpression = expressionBody.Right as MemberExpression;
					info.ValueExpression = expressionBody.Left;
					info.IsLeftToRight = false;
				}
				
				return info;
			}
		}
		struct DetailInfo
		{
			public DetailInfo(Type detailType, string valuePropertyName)
			{
				DetailType = detailType;
				ValuePropertyName = valuePropertyName;
			}

			public Type DetailType;
			public string ValuePropertyName;
		}
	}

}
