using System;
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
	internal class QueryTransformer
	{
		private readonly Engine.Logger<QueryTransformer> logger;

		static MethodInfo anyMethodInfo = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Length == 2).GetGenericMethodDefinition();
		static MethodInfo ofTypeMethodInfo = typeof(Enumerable).GetMethod("OfType").GetGenericMethodDefinition();

		#region IQueryProvider Members

		public Expression ToDetailSubselect<TElement>(Expression expression)
		{
			var expressionToReplace = expression as Expression<Func<TElement, bool>>;
			if (expressionToReplace != null)
			{
				logger.Debug("Translating: " + expression);

				ParameterExpression itemParameter = expressionToReplace.Parameters[0];
				Expression translation = Translate<TElement>(itemParameter, expressionToReplace.Body);
				var translationLambda = Expression.Lambda(translation, itemParameter);
				expression = Expression.Quote(translationLambda);
			
				logger.Debug("Into:        " + expression);
			}
			return expression;
		}

		Expression Translate<TElement>(ParameterExpression itemParameter, Expression expressionBody)
		{
			switch (expressionBody.NodeType)
			{
				case ExpressionType.Equal:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.NotEqual:
					return TranslatePropertyComparisonIntoDetailSubselect(itemParameter, expressionBody as BinaryExpression);
				case ExpressionType.AndAlso:
				case ExpressionType.OrElse:
					return CombineExpressions<TElement>(itemParameter, expressionBody as BinaryExpression);
				case ExpressionType.Call:
					return TranslateCallExpression(itemParameter, expressionBody as MethodCallExpression);
				default:
					throw new NotSupportedException("Not supported expression type " + expressionBody.NodeType + " in expression " + expressionBody);
			}
		}

		Expression TranslateCallExpression(ParameterExpression itemParameter, MethodCallExpression expressionBody)
		{
			MemberExpression member = expressionBody.Object as MemberExpression;
			DetailInfo detail = GetDetailFromPropertyType(member.Type);
			LambdaExpression nameAndValueExpression = DetailMethodCallExpression(expressionBody, detail, member.Member.Name);

			return EmbedDetailExpressionInSubselect(detail, itemParameter, nameAndValueExpression);
		}

		LambdaExpression DetailMethodCallExpression(MethodCallExpression expressionBody, DetailInfo detail, string propertyName)
		{
			ParameterExpression detailParameter = Expression.Parameter(detail.DetailType, "cd");
			var nameEqual = GetPropertyEquals(detailParameter, "Name", Expression.Constant(propertyName));

			var valueProperty = Expression.Property(detailParameter, detail.ValuePropertyName);
			var valueExpression = Expression.Call(valueProperty, expressionBody.Method, expressionBody.Arguments); // cd.StringValue.StartsWith("another")
			Expression nameAndValue = Expression.AndAlso(nameEqual, valueExpression);
			return Expression.Lambda(nameAndValue, detailParameter);
		}

		private Expression CombineExpressions<TElement>(ParameterExpression itemParameter, BinaryExpression expression)
		{
			Func<Expression, Expression, Expression> joiner;
			switch (expression.NodeType)
			{
				case ExpressionType.AndAlso:
					joiner = Expression.AndAlso;
					break;
				case ExpressionType.OrElse:
					joiner = Expression.OrElse;
					break;
				default:
					throw new NotSupportedException("Not supported expression type " + expression.NodeType + " in expression " + expression);
			}
			return joiner(Translate<TElement>(itemParameter, expression.Left), Translate<TElement>(itemParameter, expression.Right));
		}

		MethodCallExpression TranslatePropertyComparisonIntoDetailSubselect(ParameterExpression itemParameter, BinaryExpression expressionBody)
		{
			ComparisonInfo comparison = ComparisonInfo.Get(expressionBody);

			Type propertyType = GetExpressionType(comparison.NameExpression);
			var detail = GetDetailFromPropertyType(propertyType);

			var detailExpression = GetDetailExpression(comparison, detail); // cd => ((cd.Name = "StringProperty2") && (cd.StringValue = "another string"))

			return EmbedDetailExpressionInSubselect(detail, itemParameter, detailExpression);
		}

		MethodCallExpression EmbedDetailExpressionInSubselect(DetailInfo detail, ParameterExpression itemParameter, Expression nameAndValueExpression)
		{
			//var valuesProperty = Expression.Property(Expression.Property(itemParameter, "Details"), "Values"); // ci.Details.Values
			var valuesProperty = Expression.Property(itemParameter, "Details"); // ci.Detials
			//var ofTypeMethod = ofTypeMethodInfo.MakeGenericMethod(detail.DetailType); // System.Collections.Generic.IEnumerable`1[N2.Details.StringDetail] OfType[StringDetail](System.Collections.IEnumerable)
			//var ofTypeCall = Expression.Call(valuesProperty, ofTypeMethod, valuesProperty); // ci.Details.Values.OfType()
			var anyMethod = anyMethodInfo.MakeGenericMethod(detail.DetailType); // Boolean Any[StringDetail](System.Collections.Generic.IEnumerable`1[N2.Details.StringDetail], System.Func`2[N2.Details.StringDetail,System.Boolean])
			return Expression.Call(anyMethod, valuesProperty, nameAndValueExpression);
			//return Expression.Call(ofTypeCall, anyMethod, ofTypeCall, nameAndValueExpression);
		}

		private Type GetExpressionType(MemberExpression nameExpression)
		{
			if (nameExpression.Member.MemberType == MemberTypes.Property)
				return ((PropertyInfo)nameExpression.Member).PropertyType;

			throw new NotSupportedException("Comparison of " + nameExpression);
		}

		private DetailInfo GetDetailFromPropertyType(Type propertyType)
		{
			if (propertyType == typeof(string))
				return new DetailInfo(typeof(ContentDetail), "StringValue");
			if (propertyType == typeof(bool))
				return new DetailInfo(typeof(ContentDetail), "BoolValue");
			if (propertyType == typeof(int))
				return new DetailInfo(typeof(ContentDetail), "IntValue");
			if (propertyType == typeof(double))
				return new DetailInfo(typeof(ContentDetail), "DoubleValue");
			if (propertyType == typeof(DateTime))
				return new DetailInfo(typeof(ContentDetail), "DateTimeValue");
			if (typeof(ContentItem).IsAssignableFrom(propertyType))
				return new DetailInfo(typeof(ContentDetail), "LinkedItem");

			return new DetailInfo(typeof(ContentDetail), "Value");
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
			if (comparison.IsLeftToRight)
			{
				left = propertyAccess;
				right = GetValueExpression(comparison);
			}
			else
			{
				right = propertyAccess;
				left = GetValueExpression(comparison);
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

		private static Expression GetValueExpression(ComparisonInfo comparison)
		{
			if (comparison.ValueExpression.Type.IsValueType)
				return Expression.Convert(comparison.ValueExpression, typeof(Nullable<>).MakeGenericType(comparison.ValueExpression.Type));

			return comparison.ValueExpression;
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
