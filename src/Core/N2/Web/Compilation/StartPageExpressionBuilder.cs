using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Compilation;

namespace N2.Web.Compilation
{
	/// <summary>
	/// Gets a value from the current start page.
	/// </summary>
	/// <example>
	/// &lt;asp:Label text="&lt;%$ StartPage: Title %&gt;" CssClass="siteName" runat="server" /&gt;
	/// </example>
	[ExpressionPrefix("StartPage")]
	public class StartPageExpressionBuilder : N2ExpressionBuilder
	{
		/// <summary>Gets a value from the current start page.</summary>
		/// <param name="expression">The name of the value to get.</param>
		/// <returns>The value or null.</returns>
		public static object GetStartPageValue(string expression)
		{
			return N2.Find.StartPage[expression];
		}

		protected override string ExpressionFormat
		{
			get { return @"N2.Web.Compilation.StartPageExpressionBuilder.GetStartPageValue(""{0}"")"; }
		}
	}
}
