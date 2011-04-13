using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using N2.Web.Parsing;

namespace N2.Details
{
	public class DisplayableTokensAttribute : AbstractDisplayableAttribute, IWritingDisplayable, IContentTransformer
	{
		public override void Write(ContentItem item, string detailName, TextWriter writer)
		{
			throw new Exception("Should neve rhappen");
		}

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
				var p = new Parser(new Web.Wiki.Analyzers.TemplateAnalyzer());
				foreach (var c in p.Parse(text).Where(c => c.Command != Parser.TextCommand))
				{
					var dc = item.GetDetailCollection(detailName, true);
					var cd = new ContentDetail(item, detailName, c.Data) { EnclosingCollection = dc, IntValue = c.Tokens.First().Index };
					if (dc.Details.Count > i)
						dc.Details[i] = cd;
					else
						dc.Details.Add(cd);
					i++;					
				}
				if (i > 0)
				{
					var dc = item.GetDetailCollection(detailName, true);
					for (int j = dc.Details.Count - 1; j >= i; i--)
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
