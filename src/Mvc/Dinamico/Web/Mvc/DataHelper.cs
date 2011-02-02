using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2;
using System.Dynamic;

namespace N2.Web.Mvc
{
	public class DataHelper<TModel> : DynamicObject where TModel : class
	{
		HtmlHelper<TModel> html;
		ContentItem current;

		public DataHelper(HtmlHelper<TModel> html, ContentItem current)
		{
			this.html = html;
			this.current = current;
		}
		
		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return current.GetContentType().GetProperties().Select(p => p.Name);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (current == null)
			{
				result = null;
				return true;
			}

			string name = binder.Name;
			result = current[name];
			return true;
		}
	}
}