using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Definitions;

namespace N2.Details
{
	public abstract class AbstractDisplayableAttribute : Attribute, IDisplayable
	{
		private string cssClass = null;
		private string name;

		public string CssClass
		{
			get { return cssClass; }
			set { cssClass = value; }
		}

		#region IDisplayable Members
		string IUniquelyNamed.Name
		{
			get { return name; }
			set { name = value; }
		}

		public abstract Control AddTo(ContentItem item, string detailName, Control container);
		#endregion
	}
}
