using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing
{
	public abstract class AnalyzerBase : IComparable<AnalyzerBase>
	{
		public abstract Component GetComponent(IList<Token> tokens, int index);

		public int SortOrder { get; set; }

		public virtual string Name
		{
			get
			{
				string name = GetType().Name;
				if (name.EndsWith("Analyzer"))
					name = name.Substring(0, name.Length - "Analyzer".Length);
				return name;
			}

		}

		#region IComparable<AnalyzerBase> Members

		public int CompareTo(AnalyzerBase other)
		{
			return SortOrder.CompareTo(other.SortOrder);
		}

		#endregion
	}
}
