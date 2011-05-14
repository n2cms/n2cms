using System.Linq;
using N2.Web.Parsing;
using N2.Web.Wiki.Analyzers;

namespace N2.Details
{
	public class DisplayableTokensAttribute : AbstractDisplayableAttribute, IContentTransformer
	{
		#region IContentTransformer Members

		public ContentState ChangingTo
		{
			get { return ContentState.Published; }
		}

		public bool Transform(ContentItem item)
		{
			string text = item[Name] as string;
			if(text != null)
			{
				string detailName = Name + "_Tokens";
				int i = 0;
				var p = new Parser(new TemplateAnalyzer());
				foreach (var c in p.Parse(text).Where(c => c.Command != Parser.TextCommand))
				{
					var dc = item.GetDetailCollection(detailName, true);
					var cd = ContentDetail.Multi(detailName, stringValue: c.Tokens.Select(t => t.Fragment).StringJoin(), integerValue: c.Tokens.First().Index);
					cd.EnclosingItem = item;
					cd.EnclosingCollection = dc;

					if (dc.Details.Count > i)
						dc.Details[i] = cd;
					else
						dc.Details.Add(cd);
					i++;					
				}
				if (i > 0)
				{
					var dc = item.GetDetailCollection(detailName, true);
					for (int j = dc.Details.Count - 1; j >= i; j--)
					{
						dc.Details.RemoveAt(j);
					}
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}
