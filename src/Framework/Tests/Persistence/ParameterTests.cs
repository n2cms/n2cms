using N2.Persistence;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Persistence
{
	[TestFixture]
	public class ParameterTests
	{
		[Test]
		public void Equality_Equal()
		{
			var first = Parameter.Equal("Hello", "World");
			var second = Parameter.Equal("Hello", "World");

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_GreaterOrEqual()
		{
			var first = Parameter.GreaterOrEqual("Hello", 3);
			var second = Parameter.GreaterOrEqual("Hello", 3);

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_GreaterThan()
		{
			var first = Parameter.GreaterThan("Hello", 2);
			var second = Parameter.GreaterThan("Hello", 2);

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_In()
		{
			var first = Parameter.In("Hello", "World");
			var second = Parameter.In("Hello", "World");

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_NotIn()
		{
			var first = Parameter.NotIn("Hello", "World");
			var second = Parameter.NotIn("Hello", "World");

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_IsNotNull()
		{
			var first = Parameter.IsNotNull("Hello");
			var second = Parameter.IsNotNull("Hello");

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_IsNull()
		{
			var first = Parameter.IsNull("Hello");
			var second = Parameter.IsNull("Hello");

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}
		
		[Test]
		public void Equality_LessOrEqual()
		{
			var first = Parameter.LessOrEqual("Hello", 1);
			var second = Parameter.LessOrEqual("Hello", 1);

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_LessThan()
		{
			var first = Parameter.LessThan("Hello", 1);
			var second = Parameter.LessThan("Hello", 1);

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_Like()
		{
			var first = Parameter.Like("Hello", "world");
			var second = Parameter.Like("Hello", "world");

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_NotLike()
		{
			var first = Parameter.NotLike("Hello", "world");
			var second = Parameter.NotLike("Hello", "world");

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_Equal_Detail()
		{
			var first = Parameter.Equal("Hello", "World").Detail();
			var second = Parameter.Equal("Hello", "World").Detail();

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		// inequality parameters

		[Test]
		public void Inequality_In_Count()
		{
			var first = Parameter.In("Hello", "World");
			var second = Parameter.In("Hello", "World", "World");

			first.ShouldNotBe(second);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_In_Values()
		{
			var first = Parameter.In("Hello", "World");
			var second = Parameter.In("Hello", "Universe");

			first.ShouldNotBe(second);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Value()
		{
			var first = Parameter.Equal("Hello", "World");
			var second = Parameter.Equal("Hello", "Universe");

			first.ShouldNotBe(second);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Value_NullFirst()
		{
			var first = Parameter.Equal("Hello", null);
			var second = Parameter.Equal("Hello", "Universe");

			first.ShouldNotBe(second);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Value_NullSecond()
		{
			var first = Parameter.Equal("Hello", "World");
			var second = Parameter.Equal("Hello", null);

			first.ShouldNotBe(second);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Name()
		{
			var first = Parameter.Equal("Hello", "World");
			var second = Parameter.Equal("Hej", "World");

			first.ShouldNotBe(second);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Detail()
		{
			var first = Parameter.Equal("Hello", "World");
			var second = Parameter.Equal("Hello", "World").Detail();

			first.ShouldNotBe(second);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		// equality collections

		[Test]
		public void Equality_Collection_And()
		{
			var first = Parameter.Equal("Hello", "World") & Parameter.Equal("Age", 3);
			var second = Parameter.Equal("Hello", "World") & Parameter.Equal("Age", 3);

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_Collection_Or()
		{
			var first = Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3);
			var second = Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3);

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_Collection_Take()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Take(10);
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Take(10);

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_Collection_Skip()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Skip(10);
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Skip(10);

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		[Test]
		public void Equality_Collection_OrderBy()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).OrderBy("Hello DESC");
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).OrderBy("Hello DESC");

			first.ShouldBe(second);
			first.GetHashCode().ShouldBe(second.GetHashCode());
		}

		// inequality collections

		[Test]
		public void Inequality_Collection_And()
		{
			var first = Parameter.Equal("Hello", "World") & Parameter.Equal("Age", 3);
			var second = Parameter.Equal("Hello", "World") & Parameter.Equal("Age", 4);

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Count()
		{
			var first = Parameter.Equal("Hello", "World") & Parameter.Equal("Age", 3);
			var second = Parameter.Equal("Hello", "World") & Parameter.Equal("Age", 3) & Parameter.Equal("Sex", "F");

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Or()
		{
			var first = Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3);
			var second = Parameter.Equal("Hello", "Universe") | Parameter.Equal("Age", 4);

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Operator()
		{
			var first = Parameter.Equal("Hello", "World") & Parameter.Equal("Age", 3);
			var second = Parameter.Equal("Hello", "Universe") | Parameter.Equal("Age", 3);

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Take()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Take(10);
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Take(11);

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Take_First()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Take(10);
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3));

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Take_Second()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3));
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Take(11);

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Skip()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Skip(10);
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Skip(11);

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Skip_First()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Skip(10);
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3));

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_Skip_Second()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3));
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).Skip(10);

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_OrderBy()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).OrderBy("Hello DESC");
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).OrderBy("Hello");

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_OrderBy_First()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).OrderBy("Hello DESC");
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3));

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

		[Test]
		public void Inequality_Collection_OrderBy_Second()
		{
			var first = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3));
			var second = (Parameter.Equal("Hello", "World") | Parameter.Equal("Age", 3)).OrderBy("Hello");

			(first == second).ShouldBe(false);
			first.GetHashCode().ShouldNotBe(second.GetHashCode());
		}

	}
}
