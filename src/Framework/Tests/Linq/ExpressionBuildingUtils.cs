using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using N2.Details;

namespace N2.Extensions.Tests.Linq
{
    class ExpressionBuildingUtils
    {
        public void Test()
        {
            var inf = inferred();
            Output(inf);
            var constr = constructed();
            Output(constr);
        }

        void Output(Expression<Func<ContentItem, bool>> expr)
        {
            Debug.WriteLine("expr: " + expr);
            Debug.WriteLine(expr.Body);
            Debug.WriteLine(expr.Parameters[0]);

            var mce = expr.Body as MethodCallExpression;
            Debug.WriteLine("mce: " + mce);
            Debug.WriteLine(mce.Arguments.Count);
            Debug.WriteLine(mce.Arguments[0]);
            Debug.WriteLine(mce.Method);
            Debug.WriteLine(mce.Object);

            var mce2 = mce.Arguments[0] as MethodCallExpression;
            Debug.WriteLine("mce2: " + mce2);
            Debug.WriteLine(mce2.Arguments.Count);
            Debug.WriteLine(mce2.Arguments[0]);
            Debug.WriteLine(mce2.Method);
            Debug.WriteLine(mce2.Object);

            var me = mce2.Arguments[0] as MemberExpression;
            Debug.WriteLine("me: " + me);
            Debug.WriteLine(me.Expression);
            Debug.WriteLine(me.Member);

            var me2 = me.Expression as MemberExpression;
            Debug.WriteLine("me2: " + me2);
            Debug.WriteLine(me2.Expression);
            Debug.WriteLine(me2.Member);

            var pe = me2.Expression as ParameterExpression;
            Debug.WriteLine("pe: " + pe);
            Debug.WriteLine(pe.Name);
            Debug.WriteLine(pe.Type);
        }

        Expression<Func<ContentItem, bool>> constructed()
        {
            var itemParameter = Expression.Parameter(typeof(ContentItem), "ci");
            var detailsProperty = Expression.Property(itemParameter, "Details");
            var valuesProperty = Expression.Property(detailsProperty, "Values");
            var ofTypeMethod = typeof(Enumerable).GetMethod("OfType").GetGenericMethodDefinition().MakeGenericMethod(typeof(ContentDetail));
            var ofTypeCall = Expression.Call(valuesProperty, ofTypeMethod, valuesProperty);
            var anyMethod = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Length == 2).GetGenericMethodDefinition().MakeGenericMethod(typeof(ContentDetail));

            var cdParameter = Expression.Parameter(typeof(ContentDetail), "cd");
            var stringValueProperty = Expression.Property(cdParameter, "StringValue");
            var constant = Expression.Constant("hello");
            var equalsExpression = Expression.Equal(stringValueProperty, constant);
            var anyExpression = Expression.Lambda<Func<ContentDetail, bool>>(equalsExpression, cdParameter);
            var anyCall = Expression.Call(ofTypeCall, anyMethod, ofTypeCall, anyExpression);
            // ---
            return Expression.Lambda<Func<ContentItem, bool>>(anyCall, itemParameter);
        }

        Expression<Func<ContentItem, bool>> inferred()
        {
            return ci => ci.Details.Any(cd => cd.StringValue == "hello");
        }
    }
}
